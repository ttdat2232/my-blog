namespace MyBlog.Core.Constants;

public static class WebSocketBrokerConstants
{
    public const string WEBSOCKET_EXCHANGE = "websocket.exchange";
    public const string USER_NOTIFICATION_QUEUE = "websocket.user.notifications";
    public const string BROADCAST_QUEUE = "websocket.broadcast";
    public const string USER_NOTIFICATION_ROUTING_KEY = "websocket.user";
    public const string BROADCAST_ROUTING_KEY = "websocket.broadcast";
}
