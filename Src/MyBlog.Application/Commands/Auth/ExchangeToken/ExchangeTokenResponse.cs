namespace MyBlog.Application.Commands.Auth.ExchangeToken;

public record ExchangeTokenResponse(string AccessToken, string RefreshToken, int ExpiresIn);
