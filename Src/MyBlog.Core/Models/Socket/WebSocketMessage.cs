namespace MyBlog.Core.Models.Socket;

public record WebSocketMessage(Guid UserId, object Message);
