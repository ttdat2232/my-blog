namespace MyBlog.RabbitMq.Configurations;

public class RabbitMqConfiguration
{
    /// <summary>
    /// Connection string to the message broker.
    /// </summary>
    public string ConnectionString { get; set; } = null!;

    /// <summary>
    /// Maximum number of retry attempts for publishing or consuming messages.
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Timeout in milliseconds for message operations.
    /// </summary>
    public int OperationTimeoutMs { get; set; } = 30000;

    /// <summary>
    /// Prefetch count for consuming messages.
    /// </summary>
    public ushort PrefetchCount { get; set; } = 10;
}
