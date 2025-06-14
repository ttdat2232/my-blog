using System.Net.WebSockets;
using MyBlog.Core.Aggregates.Users;

namespace MyBlog.Core.Services.Socket;

public interface IWebSocketManager
{
    Task AddConnectionAsync(
        string connectionId,
        WebSocket webSocket,
        UserId userId,
        CancellationToken cancellationToken = default
    );
    Task RemoveConnectionAsync(string connectionId, CancellationToken cancellationToken = default);
    Task<WebSocket?> GetConnection(
        string connectionId,
        CancellationToken cancellationToken = default
    );
    Task<IEnumerable<string>> GetConnectionIds(CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetConnectionsByUserId(
        string userId,
        CancellationToken cancellationToken = default
    );
    Task SendMessageAsync(
        string connectionId,
        string message,
        CancellationToken cancellationToken = default
    );
    Task SendMessageToUserAsync(
        string userId,
        string message,
        CancellationToken cancellationToken = default
    );
    Task SendMessageToAllAsync(string message, CancellationToken cancellationToken = default);
    bool IsConnected(string connectionId);
}
