namespace MyBlog.Auth.Models.Auth;

public record LoginRequest(string UsernameOrEmail, string Password);
