using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using MyBlog.Core.Repositories;
using MyBlog.Jwt.Configurations;
using MyBlog.Jwt.Models;

namespace MyBlog.Jwt.Repositories;

public class TokenRepository : ITokenRepository
{
    private readonly ConcurrentDictionary<string, RefreshTokenInfo> _tokens = new();
    private readonly JwtSettings _jwtSettings;

    public TokenRepository(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<StoredToken?> GetRefreshTokenAsync(string refreshToken)
    {
        if (
            _tokens.TryGetValue(refreshToken, out var tokenInfo)
            && !tokenInfo.IsRevoked
            && tokenInfo.ExpiresAt > DateTime.UtcNow
        )
        {
            return new StoredToken(
                tokenInfo.UserId,
                tokenInfo.Username,
                tokenInfo.Email,
                tokenInfo.RefreshToken
            );
        }

        return null;
    }

    public Task RevokeAllTokensAsync(Guid userId)
    {
        var userTokens = _tokens.Where(t => t.Value.UserId == userId);
        foreach (var token in userTokens)
        {
            if (_tokens.TryGetValue(token.Key, out var tokenInfo))
            {
                tokenInfo.IsRevoked = true;
            }
        }

        return Task.CompletedTask;
    }

    public Task RevokeUserTokensAsync(Guid userId)
    {
        return RevokeAllTokensAsync(userId);
    }

    public Task StoreRefreshTokenAsync(Guid userId, string refreshToken)
    {
        var tokenInfo = new RefreshTokenInfo
        {
            UserId = userId,
            RefreshToken = refreshToken,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays),
            IsRevoked = false,
        };

        _tokens.TryAdd(refreshToken, tokenInfo);

        return Task.CompletedTask;
    }
}
