namespace MyBlog.WebApi.Models.Auth;

public record LoginRequest(string UsernameOrEmail, string Password);
