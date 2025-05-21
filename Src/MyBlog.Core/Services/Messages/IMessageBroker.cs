using MyBlog.Core.Models.Messages;

namespace MyBlog.Core.Services.Messages;

public interface IMessageBroker : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Initializes the message broker with the provided options.
    /// </summary>
    /// <param name="options">Configuration options for the message broker.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task InitializeAsync();

    /// <summary>
    /// Creates an exchange/topic if it doesn't exist.
    /// </summary>
    /// <param name="exchangeName">Name of the exchange/topic.</param>
    /// <param name="exchangeType">Type of the exchange (direct, fanout, topic, etc.).</param>
    /// <param name="durable">Whether the exchange should survive broker restarts.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CreateExchangeAsync(string exchangeName, string exchangeType, bool durable = true);

    /// <summary>
    /// Creates a queue if it doesn't exist.
    /// </summary>
    /// <param name="queueName">Name of the queue.</param>
    /// <param name="durable">Whether the queue should survive broker restarts.</param>
    /// <param name="exclusive">Whether the queue should be used only by one connection.</param>
    /// <param name="autoDelete">Whether the queue should be deleted when no longer used.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CreateQueueAsync(
        string queueName,
        bool durable = true,
        bool exclusive = false,
        bool autoDelete = false
    );

    /// <summary>
    /// Binds a queue to an exchange with a routing key.
    /// </summary>
    /// <param name="queueName">Name of the queue.</param>
    /// <param name="exchangeName">Name of the exchange.</param>
    /// <param name="routingKey">Routing key for the binding.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task BindQueueAsync(string queueName, string exchangeName, string routingKey);

    /// <summary>
    /// Publishes a message to an exchange with a routing key.
    /// </summary>
    /// <typeparam name="T">Type of the message payload.</typeparam>
    /// <param name="message">Message to publish.</param>
    /// <param name="exchangeName">Name of the exchange.</param>
    /// <param name="routingKey">Routing key for the message.</param>
    /// <param name="persistent">Whether the message should be persisted.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task PublishAsync<T>(
        Message<T> message,
        string exchangeName,
        string routingKey,
        bool persistent = true,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Subscribes to a queue and processes messages.
    /// </summary>
    /// <typeparam name="T">Type of the message payload.</typeparam>
    /// <param name="queueName">Name of the queue.</param>
    /// <param name="messageHandler">Handler to process messages.</param>
    /// <param name="autoAck">Whether to automatically acknowledge messages.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SubscribeAsync<T>(
        string queueName,
        Func<Message<T>, Task<bool>> messageHandler,
        bool autoAck = false,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Acknowledges a message.
    /// </summary>
    /// <param name="messageId">ID of the message to acknowledge.</param>
    /// <param name="multiple">Whether to acknowledge multiple messages up to this one.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AcknowledgeAsync(string messageId, bool multiple = false);

    /// <summary>
    /// Rejects or negatively acknowledges a message.
    /// </summary>
    /// <param name="messageId">ID of the message to reject.</param>
    /// <param name="requeue">Whether to requeue the message.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RejectAsync(string messageId, bool requeue = true);

    /// <summary>
    /// Consumes a single message from a queue.
    /// </summary>
    /// <typeparam name="T">Type of the message payload.</typeparam>
    /// <param name="queueName">Name of the queue.</param>
    /// <param name="timeout">Timeout in milliseconds.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A message if available, otherwise null.</returns>
    Task<Message<T>> ConsumeAsync<T>(
        string queueName,
        int timeout = 5000,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Creates a dead letter queue for handling failed messages.
    /// </summary>
    /// <param name="queueName">Name of the queue.</param>
    /// <param name="deadLetterExchange">Name of the dead letter exchange.</param>
    /// <param name="deadLetterRoutingKey">Routing key for the dead letter queue.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CreateDeadLetterQueueAsync(
        string queueName,
        string deadLetterExchange,
        string deadLetterRoutingKey
    );

    /// <summary>
    /// Purges all messages from a queue.
    /// </summary>
    /// <param name="queueName">Name of the queue to purge.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task PurgeQueueAsync(string queueName);

    /// <summary>
    /// Deletes a queue.
    /// </summary>
    /// <param name="queueName">Name of the queue to delete.</param>
    /// <param name="ifUnused">Delete only if the queue has no consumers.</param>
    /// <param name="ifEmpty">Delete only if the queue is empty.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteQueueAsync(string queueName, bool ifUnused = false, bool ifEmpty = false);

    /// <summary>
    /// Deletes an exchange.
    /// </summary>
    /// <param name="exchangeName">Name of the exchange to delete.</param>
    /// <param name="ifUnused">Delete only if the exchange has no bindings.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteExchangeAsync(string exchangeName, bool ifUnused = false);

    /// <summary>
    /// Gets the number of messages in a queue.
    /// </summary>
    /// <param name="queueName">Name of the queue.</param>
    /// <returns>The number of messages in the queue.</returns>
    Task<uint> GetMessageCountAsync(string queueName);

    /// <summary>
    /// Gets the health status of the message broker.
    /// </summary>
    /// <returns>True if the message broker is healthy, otherwise false.</returns>
    Task<bool> GetHealthStatusAsync();
}
