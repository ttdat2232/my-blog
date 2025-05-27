namespace MyBlog.Auth.Models.Auth;

public record RegisterRequest(string Username, string Email, string Password, string SessionId);
