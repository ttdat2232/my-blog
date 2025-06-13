using System.Net.WebSockets;
using MyBlog.Core.Aggregates.Users;
using MyBlog.Core.Services.Socket;

namespace MyBlog.Application.Services;

public class WebSocketHandler(
    IWebSocketManager _connectionManager,
    IMessageProcessor _messageProcessor
) : IWebSocketHandler
{
    public async Task HandleAsync(WebSocket webSocket, string connectionId)
    {
        var buffer = new byte[1024 * 4];

        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    CancellationToken.None
                );

                if (result.MessageType == System.Net.WebSockets.WebSocketMessageType.Text)
                {
                    var message = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
                    await OnMessageReceivedAsync(connectionId, message);
                }
                else if (result.MessageType == System.Net.WebSockets.WebSocketMessageType.Close)
                {
                    await OnDisconnectedAsync(connectionId);
                    break;
                }
            }
        }
        catch (WebSocketException ex)
        {
            Serilog.Log.Error(ex, "WebSocket error for connection {ConnectionId}", connectionId);
            await OnDisconnectedAsync(connectionId);
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(
                ex,
                "Unexpected error handling WebSocket connection {ConnectionId}",
                connectionId
            );
            await OnDisconnectedAsync(connectionId);
        }
    }

    public async Task OnConnectedAsync(string connectionId, UserId userId)
    {
        Serilog.Log.Information("Client {ConnectionId} connected", connectionId);

        var welcomeMessage = new
        {
            Type = "connection",
            Content = "Connected successfully",
            Success = true,
            Data = new Dictionary<string, object> { ["connectionId"] = connectionId },
        };

        await _connectionManager.SendMessageAsync(
            connectionId,
            System.Text.Json.JsonSerializer.Serialize(welcomeMessage)
        );
    }

    public async Task OnDisconnectedAsync(string connectionId)
    {
        await _connectionManager.RemoveConnectionAsync(connectionId);
        Serilog.Log.Information("Client {ConnectionId} disconnected", connectionId);
    }

    public async Task OnMessageReceivedAsync(string connectionId, object message)
    {
        Serilog.Log.Debug("Received message from {ConnectionId}: {Message}", connectionId, message);

        var processed = await _messageProcessor.ProcessMessageAsync(
            connectionId,
            message,
            Core.Services.Socket.WebSocketMessageType.Private
        );

        if (processed.IsFailure)
        {
            var errorResponse = new
            {
                Type = "error",
                Content = "Failed to process message",
                Success = false,
                Error = "Invalid message format",
            };

            await _connectionManager.SendMessageAsync(
                connectionId,
                System.Text.Json.JsonSerializer.Serialize(errorResponse)
            );
        }
    }
}
