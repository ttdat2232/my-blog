namespace MyBlog.Core.Models.Messages;

public class Message<T>
{
    /// <summary>
    /// Message payload.
    /// </summary>
    public T Payload { get; set; }

    /// <summary>
    /// Message metadata.
    /// </summary>
    public MessageMetadata Metadata { get; set; } = new MessageMetadata();
}
