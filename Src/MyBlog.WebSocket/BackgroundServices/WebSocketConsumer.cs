using System.Reflection.Metadata;
using MyBlog.Core.Constants;
using MyBlog.Core.Models.Messages;
using MyBlog.Core.Models.Socket;
using MyBlog.Core.Services.Messages;
using MyBlog.Core.Services.Socket;

namespace MyBlog.WebSocket.BackgroundServices;

public class WebSocketConsumer(IMessageBroker _messageBroker, IWebSocketManager _socketManager)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Serilog.Log.Debug(
            "Setup websocket background service for listening data from message queue"
        );
        await _messageBroker.CreateQueueAsync(WebSocketBrokerConstants.USER_NOTIFICATION_QUEUE);
        await _messageBroker.BindQueueAsync(
            WebSocketBrokerConstants.USER_NOTIFICATION_QUEUE,
            WebSocketBrokerConstants.WEBSOCKET_EXCHANGE,
            WebSocketBrokerConstants.USER_NOTIFICATION_ROUTING_KEY
        );
        await _messageBroker.SubscribeAsync<WebSocketMessage>(
            WebSocketBrokerConstants.USER_NOTIFICATION_QUEUE,
            msg => Handle(msg, stoppingToken),
            autoAck: true,
            cancellationToken: stoppingToken
        );
    }

    private async Task<bool> Handle(
        Message<WebSocketMessage> message,
        CancellationToken cancellationToken
    )
    {
        try
        {
            Serilog.Log.Debug(
                "Websocket recieved message sent to user {UserId}",
                message.Payload.UserId
            );
            var payload = message.Payload;
            await _socketManager.SendMessageToUserAsync(
                payload.UserId.ToString(),
                System.Text.Json.JsonSerializer.Serialize(payload.Message),
                cancellationToken
            );
            return true;
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "Handle websocket message error");
            return false;
        }
    }
}
