using System.Text;
using Microsoft.Extensions.Options;
using MyBlog.Core.Models.Messages;
using MyBlog.Core.Services.Cache;
using MyBlog.Core.Services.Messages;
using MyBlog.RabbitMq.Configurations;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MyBlog.RabbitMq;

/// <summary>
/// RabbitMQ implementation of IMessageBroker
/// </summary>
public class RabbitMqMessageBroker : IMessageBroker
{
    private readonly IMessageSerializer _serializer;
    private readonly IOptions<RabbitMqConfiguration> _configOpts;
    private IConnection _connection = null!;
    private IChannel _channel = null!;
    private readonly ICacheService _cacheService;

    private bool _disposed;

    public RabbitMqMessageBroker(
        IMessageSerializer serializer,
        IOptions<RabbitMqConfiguration> configOpts,
        ICacheService cacheService
    )
    {
        _serializer = serializer;
        _configOpts = configOpts;
        _cacheService = cacheService;
        InitializeAsync().Wait();
    }

    public async Task InitializeAsync()
    {
        if (_connection != null)
        {
            try
            {
                if (_channel?.IsOpen == true)
                {
                    await _channel.CloseAsync();
                    _channel.Dispose();
                }

                if (_connection?.IsOpen == true)
                {
                    await _connection.CloseAsync();
                    _connection.Dispose();
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Error closing existing RabbitMQ connections");
            }
        }

        var factory = new ConnectionFactory
        {
            Uri = new Uri(_configOpts.Value.ConnectionString),
            RequestedHeartbeat = TimeSpan.FromSeconds(30),
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
        };

        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
        await _channel.BasicQosAsync(0, _configOpts.Value.PrefetchCount, false);

        Serilog.Log.Information("RabbitMQ connection established");
    }

    public async Task AcknowledgeAsync(string messageId, bool multiple = false)
    {
        var result = await _cacheService.GetAndRemoveAsync<BasicGetResult>(
            $"rabbitmq:message:{messageId}"
        );

        if (result != null)
        {
            await _channel.BasicAckAsync(result.DeliveryTag, multiple);
        }
        else
        {
            Serilog.Log.Warning("Attempted to acknowledge unknown message: {MessageId}", messageId);
        }
    }

    public async Task BindQueueAsync(string queueName, string exchangeName, string routingKey)
    {
        await _channel.QueueBindAsync(queueName, exchangeName, routingKey);
    }

    public async Task<Message<T>> ConsumeAsync<T>(
        string queueName,
        int timeout = 5000,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _channel.BasicGetAsync(queueName, false, cancellationToken);

        if (result == null)
        {
            return null;
        }

        try
        {
            var payload = _serializer.Deserialize<T>(result.Body.ToArray());
            var props = result.BasicProperties;

            var metadata = new MessageMetadata
            {
                MessageId = props.MessageId ?? Guid.NewGuid().ToString(),
                MessageType = props.Type,
                Timestamp =
                    props.Timestamp.UnixTime > 0
                        ? DateTimeOffset.FromUnixTimeMilliseconds(props.Timestamp.UnixTime).DateTime
                        : DateTime.UtcNow,
                CorrelationId = props.CorrelationId,
            };

            if (props.Headers != null)
            {
                foreach (var header in props.Headers)
                {
                    if (header.Value is byte[] bytes)
                    {
                        metadata.Headers[header.Key] = Encoding.UTF8.GetString(bytes);
                    }
                    else
                    {
                        metadata.Headers[header.Key] = header.Value?.ToString();
                    }
                }
            }

            var message = new Message<T> { Payload = payload, Metadata = metadata };

            await _cacheService.SetAsync($"rabbitmq:message:{metadata.MessageId}", result);
            return message;
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "Error consuming message from queue: {QueueName}", queueName);
            await _channel.BasicRejectAsync(result.DeliveryTag, true);
            return null;
        }
    }

    public async Task CreateDeadLetterQueueAsync(
        string queueName,
        string deadLetterExchange,
        string deadLetterRoutingKey
    )
    {
        var dlQueueName = $"{queueName}.deadletter";
        var arguments = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", deadLetterExchange },
            { "x-dead-letter-routing-key", deadLetterRoutingKey },
        };

        await _channel.ExchangeDeclareAsync(deadLetterExchange, ExchangeType.Direct, true);
        await _channel.QueueDeclareAsync(dlQueueName, true, false, false, null);
        await _channel.QueueBindAsync(dlQueueName, deadLetterExchange, deadLetterRoutingKey);

        await _channel.QueueDeclareAsync(queueName, true, false, false, arguments);
    }

    public async Task CreateExchangeAsync(
        string exchangeName,
        string exchangeType,
        bool durable = true
    )
    {
        await _channel.ExchangeDeclareAsync(exchangeName, exchangeType, durable);
    }

    public async Task CreateQueueAsync(
        string queueName,
        bool durable = true,
        bool exclusive = false,
        bool autoDelete = false
    )
    {
        await _channel.QueueDeclareAsync(queueName, durable, exclusive, autoDelete, null);
    }

    public async Task DeleteExchangeAsync(string exchangeName, bool ifUnused = false)
    {
        await _channel.ExchangeDeleteAsync(exchangeName, ifUnused);
    }

    public async Task DeleteQueueAsync(
        string queueName,
        bool ifUnused = false,
        bool ifEmpty = false
    )
    {
        await _channel.QueueDeleteAsync(queueName, ifUnused, ifEmpty);
    }

    public Task<bool> GetHealthStatusAsync()
    {
        return Task.FromResult(_channel?.IsOpen == true && _connection?.IsOpen == true);
    }

    public async Task<uint> GetMessageCountAsync(string queueName)
    {
        var response = await _channel.QueueDeclarePassiveAsync(queueName);
        return response.MessageCount;
    }

    public async Task PublishAsync<T>(
        Message<T> message,
        string exchangeName,
        string routingKey,
        bool persistent = true,
        CancellationToken cancellationToken = default
    )
    {
        var properties = new BasicProperties();
        properties.Persistent = persistent;
        properties.MessageId = message.Metadata.MessageId ?? Guid.NewGuid().ToString();
        properties.CorrelationId = message.Metadata.CorrelationId ?? GerateCorrelationId();
        properties.Type = message.Metadata.MessageType ?? typeof(T).Name;
        properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        properties.Headers = new Dictionary<string, object?>();

        foreach (var header in message.Metadata.Headers)
        {
            properties.Headers[header.Key] = header.Value;
        }

        var body = _serializer.Serialize(message.Payload);
        await _channel.BasicPublishAsync(exchangeName, routingKey, false, properties, body);
    }

    public async Task PurgeQueueAsync(string queueName)
    {
        await _channel.QueuePurgeAsync(queueName);
    }

    public async Task RejectAsync(string messageId, bool requeue = true)
    {
        var result = await _cacheService.GetAndRemoveAsync<BasicGetResult>(
            $"rabbitmq:message:{messageId}"
        );
        if (result != null)
        {
            await _channel.BasicRejectAsync(result.DeliveryTag, requeue);
        }
        else
        {
            Serilog.Log.Warning("Attempted to reject unknown message: {MessageId}", messageId);
        }
    }

    public async Task SubscribeAsync<T>(
        string queueName,
        Func<Message<T>, Task<bool>> messageHandler,
        bool autoAck = false,
        CancellationToken cancellationToken = default
    )
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (sender, args) =>
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                var payload = _serializer.Deserialize<T>(args.Body.ToArray());
                var props = args.BasicProperties;

                var metadata = new MessageMetadata
                {
                    MessageId = props.MessageId ?? Guid.NewGuid().ToString(),
                    MessageType = props.Type!,
                    Timestamp =
                        props.Timestamp.UnixTime > 0
                            ? DateTimeOffset
                                .FromUnixTimeMilliseconds(props.Timestamp.UnixTime)
                                .DateTime
                            : DateTime.UtcNow,
                    CorrelationId = props.CorrelationId!,
                };

                if (props.Headers != null)
                {
                    foreach (var header in props.Headers)
                    {
                        if (header.Value is byte[] bytes)
                        {
                            metadata.Headers[header.Key] = Encoding.UTF8.GetString(bytes);
                        }
                        else
                        {
                            metadata.Headers[header.Key] = header.Value?.ToString() ?? "";
                        }
                    }
                }

                var message = new Message<T> { Payload = payload!, Metadata = metadata };

                await _cacheService.SetAsync(
                    $"rabbitmq:message:{metadata.MessageId}",
                    new BasicGetResult(
                        args.DeliveryTag,
                        args.Redelivered,
                        args.Exchange,
                        args.RoutingKey,
                        0,
                        args.BasicProperties,
                        args.Body
                    )
                );

                var success = await messageHandler(message);

                if (autoAck)
                {
                    if (success)
                    {
                        await _channel.BasicAckAsync(args.DeliveryTag, false);
                    }
                    else
                    {
                        await _channel.BasicRejectAsync(args.DeliveryTag, true);
                    }
                    _ = _cacheService.RemoveAsync($"rabbitmq:message:{metadata.MessageId}");
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Error processing RabbitMQ message");
                await _channel.BasicRejectAsync(args.DeliveryTag, true);
            }
        };

        var tag = await _channel.BasicConsumeAsync(queueName, false, consumer);
        await _cacheService.SetAsync($"rabbitmq:consumingtag:{queueName}", tag);

        var tcs = new TaskCompletionSource<bool>();
        using (cancellationToken.Register(() => tcs.TrySetResult(true)))
        {
            await tcs.Task;

            var consumerTag = await _cacheService.GetAndRemoveAsync<string>(
                $"rabbitmq:consumingtag:{queueName}"
            );
            if (consumerTag != null)
            {
                await _channel.BasicCancelAsync(consumerTag);
            }
        }
    }

    public async void Dispose()
    {
        await Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual async Task Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            await CleanUp();
        }

        _disposed = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;
        await CleanUp();
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    private async Task CleanUp()
    {
        try
        {
            _ = _cacheService.RemoveAllByKeyPatternAsync<BasicGetResult>("rabbitmq:message:");
            var _consumingTags = await _cacheService.RemoveAllByKeyPatternAsync<string>(
                "rabbitmq:consumingtag:",
                true
            );
            foreach (var tag in _consumingTags)
            {
                if (_channel?.IsOpen == true)
                {
                    await _channel.BasicCancelAsync(tag);
                }
            }
            if (_channel != null)
            {
                await _channel.CloseAsync();
                _channel.Dispose();
            }
            if (_connection != null)
            {
                await _connection.CloseAsync();
                _connection.Dispose();
            }
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "Error disposing RabbitMQ connection asynchronously");
        }
    }

    private static string GerateCorrelationId() =>
        $"{Guid.NewGuid()}-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
}
