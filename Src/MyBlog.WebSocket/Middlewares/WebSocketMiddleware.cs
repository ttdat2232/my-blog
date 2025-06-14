using MyBlog.Core.Aggregates.Users;
using MyBlog.Core.Services.Socket;
using MyBlog.WebSocket.Extensions;

namespace MyBlog.WebSocket.Middlewares;

public class WebSocketMiddleware
{
    private readonly RequestDelegate _next;

    public WebSocketMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        using var scope = context.RequestServices.CreateScope();
        var _connectionManager = scope.ServiceProvider.GetRequiredService<IWebSocketManager>();
        var _webSocketHandler = scope.ServiceProvider.GetRequiredService<IWebSocketHandler>();
        var userId = UserId.From(context.User.GetUserId());
        if (context.Request.Path == "/ws" && context.WebSockets.IsWebSocketRequest)
        {
            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var connectionId = Guid.NewGuid().ToString();
            await _connectionManager.AddConnectionAsync(connectionId, webSocket, userId);
            await _webSocketHandler.OnConnectedAsync(connectionId, userId);
            try
            {
                await _webSocketHandler.HandleAsync(webSocket, connectionId);
            }
            finally
            {
                await _webSocketHandler.OnDisconnectedAsync(connectionId);
            }
        }
        else
        {
            await _next(context);
        }
    }
}
