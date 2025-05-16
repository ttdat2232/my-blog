namespace MyBlog.Application.Commands.Auth.RefreshToken;

public record RefreshTokenResponse(string RefreshToken, string AccessToken, int ExpireIn);
