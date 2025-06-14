using System.Net.WebSockets;
using MyBlog.Core.Aggregates.Users;

namespace MyBlog.Core.Services.Socket;

public interface IWebSocketHandler
{
    Task HandleAsync(
        WebSocket webSocket,
        string connectionId,
        CancellationToken cancellationToken = default
    );
    Task OnConnectedAsync(
        string connectionId,
        UserId userId,
        CancellationToken cancellationToken = default
    );
    Task OnDisconnectedAsync(string connectionId, CancellationToken cancellationToken = default);
    Task OnMessageReceivedAsync(
        string connectionId,
        object message,
        CancellationToken cancellationToken = default
    );
}
