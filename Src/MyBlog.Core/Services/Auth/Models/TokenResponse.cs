namespace MyBlog.Core.Services.Auth.Models;

public record TokenResponse(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    string TokenType = "Bearer"
);
