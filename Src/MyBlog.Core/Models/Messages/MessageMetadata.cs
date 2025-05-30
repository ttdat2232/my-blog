namespace MyBlog.Core.Models.Messages;

public class MessageMetadata
{
    /// <summary>
    /// Unique identifier for the message.
    /// </summary>
    public string? MessageId { get; set; }

    /// <summary>
    /// Timestamp when the message was created.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Type of the message.
    /// </summary>
    public string? MessageType { get; set; }

    /// <summary>
    /// Correlation ID for tracing related messages.
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Additional headers or properties.
    /// </summary>
    public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
}
