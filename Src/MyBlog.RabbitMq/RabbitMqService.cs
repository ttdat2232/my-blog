using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MyBlog.Core.Models.Messages;
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
    private readonly ConcurrentDictionary<string, string> _consumingTags =
        new ConcurrentDictionary<string, string>();
    private readonly ConcurrentDictionary<string, BasicGetResult> _messageRefs =
        new ConcurrentDictionary<string, BasicGetResult>();
    private bool _disposed;

    public RabbitMqMessageBroker(
        IMessageSerializer serializer,
        IOptions<RabbitMqConfiguration> configOpts
    )
    {
        _serializer = serializer;
        _configOpts = configOpts;
    }

    public async Task InitializeAsync()
    {
        // Ensure closure of connection
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
        if (_messageRefs.TryGetValue(messageId, out var result))
        {
            await _channel.BasicAckAsync(result.DeliveryTag, multiple);
            _messageRefs.TryRemove(messageId, out _);
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

            _messageRefs[metadata.MessageId] = result;
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

    public void Dispose()
    {
        Dispose(true).Wait();
        GC.SuppressFinalize(this);
    }

    protected virtual async Task Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            try
            {
                foreach (var tag in _consumingTags)
                {
                    if (_channel?.IsOpen == true)
                    {
                        await _channel.BasicCancelAsync(tag.Value);
                    }
                }

                _consumingTags.Clear();
                _messageRefs.Clear();

                await _channel?.CloseAsync();
                _channel?.Dispose();
                await _connection?.CloseAsync();
                _connection?.Dispose();
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Error disposing RabbitMQ connection");
            }
        }

        _disposed = true;
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
        properties.CorrelationId = message.Metadata.CorrelationId;
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
        if (_messageRefs.TryGetValue(messageId, out var result))
        {
            await _channel.BasicRejectAsync(result.DeliveryTag, requeue);
            _messageRefs.TryRemove(messageId, out _);
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

                _messageRefs[metadata.MessageId] = new BasicGetResult(
                    args.DeliveryTag,
                    args.Redelivered,
                    args.Exchange,
                    args.RoutingKey,
                    0,
                    args.BasicProperties,
                    args.Body
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
                    _messageRefs.TryRemove(metadata.MessageId, out _);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Error processing RabbitMQ message");
                await _channel.BasicRejectAsync(args.DeliveryTag, true);
            }
        };

        var tag = await _channel.BasicConsumeAsync(queueName, false, consumer);
        _consumingTags[queueName] = tag;

        // Keep this running until cancelled
        var tcs = new TaskCompletionSource<bool>();
        using (cancellationToken.Register(() => tcs.TrySetResult(true)))
        {
            await tcs.Task;

            // Cancel consumer
            if (_consumingTags.TryRemove(queueName, out var consumerTag))
            {
                await _channel.BasicCancelAsync(consumerTag);
            }
        }
    }
}
