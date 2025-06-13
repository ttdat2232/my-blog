using System.Net.WebSockets;
using MyBlog.Core.Aggregates.Users;

namespace MyBlog.Core.Services.Socket;

public interface IWebSocketManager
{
    Task AddConnectionAsync(string connectionId, WebSocket webSocket, UserId userId);
    Task RemoveConnectionAsync(string connectionId);
    Task<WebSocket?> GetConnection(string connectionId);
    Task<IEnumerable<string>> GetConnectionIds();
    Task<IEnumerable<string>> GetConnectionsByUserId(string userId);
    Task SendMessageAsync(string connectionId, string message);
    Task SendMessageToUserAsync(string userId, string message);

    Task SendMessageToAllAsync(string message);
    bool IsConnected(string connectionId);
    // Task SendMessageToRoomAsync(string roomId, string message);
}
