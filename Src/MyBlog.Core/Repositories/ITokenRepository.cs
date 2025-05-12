namespace MyBlog.Core.Repositories;

public interface ITokenRepository
{
    Task StoreRefreshTokenAsync(Guid userId, string refreshToken);
    Task<StoredToken?> GetRefreshTokenAsync(string refreshToken);
    Task RevokeUserTokensAsync(Guid userId);
    Task RevokeAllTokensAsync(Guid userId);
}

public record StoredToken(Guid UserId, string Username, string Email, string RefreshToken);
