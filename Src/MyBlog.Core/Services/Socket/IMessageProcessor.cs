using MyBlog.Core.Models;

namespace MyBlog.Core.Services.Socket;

public interface IMessageProcessor
{
    Task<Result<bool>> ProcessMessageAsync(
        string connectionId,
        object rawMessage,
        WebSocketMessageType type,
        CancellationToken cancellationToken = default
    );
}
