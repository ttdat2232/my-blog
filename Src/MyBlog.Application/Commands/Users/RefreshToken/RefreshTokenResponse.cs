namespace MyBlog.Application.Commands.Users.RefreshToken;

public record RefreshTokenResponse(string RefreshToken, string AccessToken, int ExpireIn);
