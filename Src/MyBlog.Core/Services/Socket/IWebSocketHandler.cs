using System.Net.WebSockets;
using MyBlog.Core.Aggregates.Users;

namespace MyBlog.Core.Services.Socket;

public interface IWebSocketHandler
{
    Task HandleAsync(WebSocket webSocket, string connectionId);
    Task OnConnectedAsync(string connectionId, UserId userId);
    Task OnDisconnectedAsync(string connectionId);
    Task OnMessageReceivedAsync(string connectionId, object message);
}
