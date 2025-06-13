using MyBlog.Core.Models;
using MyBlog.Core.Services.Socket;

namespace MyBlog.Application.Services;

public class MessageProccessor(IWebSocketManager _manager) : IMessageProcessor
{
    async Task<Result<bool>> IMessageProcessor.ProcessMessageAsync(
        string connectionId,
        object rawMessage,
        WebSocketMessageType type
    )
    {
        try
        {
            var message = System.Text.Json.JsonSerializer.Serialize(rawMessage);

            switch (type)
            {
                case WebSocketMessageType.Boarcast:
                    await _manager.SendMessageToAllAsync(message);
                    break;
                case WebSocketMessageType.Private:
                    await _manager.SendMessageAsync(connectionId, message);
                    break;
            }
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(
                ex,
                "Error occurred when proccessing message for connection {ConnectionId}: {Message}",
                connectionId,
                ex.Message
            );
            return Result<bool>.Failure(
                new($"Error occurred when proccessing message for connection {connectionId}", 500)
            );
        }
    }
}
