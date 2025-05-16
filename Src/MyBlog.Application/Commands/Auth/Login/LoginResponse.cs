namespace MyBlog.Application.Commands.Auth.Login;

public record LoginResponse(string AuthorizationCode, string RedirectUri);
