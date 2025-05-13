using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using MyBlog.Core.Repositories;
using MyBlog.Core.Services.Cache;
using MyBlog.Jwt.Configurations;
using MyBlog.Jwt.Models;

namespace MyBlog.Jwt.Repositories;

public class TokenRepository : ITokenRepository
{
    private readonly ICacheService _cache;
    private readonly ICacheKeyProvider _keyProvider;
    private readonly JwtSettings _jwtSettings;

    public TokenRepository(
        IOptions<JwtSettings> jwtSettings,
        ICacheService cache,
        ICacheKeyProvider keyProvider
    )
    {
        _jwtSettings = jwtSettings.Value;
        _cache = cache;
        _keyProvider = keyProvider;
    }

    public async Task<StoredToken?> GetRefreshTokenAsync(string refreshToken)
    {
        var tokenInfo = await _cache.GetAsync<RefreshTokenInfo>(refreshToken);

        if (tokenInfo != null && !tokenInfo.IsRevoked && tokenInfo.ExpiresAt > DateTime.UtcNow)
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

    public async Task RevokeAllTokensAsync(Guid userId)
    {
        string userTokensKey = _keyProvider.GenerateKey("UserTokens", userId.ToString());
        var userTokenIds = await _cache.GetAsync<List<string>>(userTokensKey);

        if (userTokenIds != null)
        {
            // Mark each token as revoked
            foreach (var tokenId in userTokenIds)
            {
                var token = await _cache.GetAsync<RefreshTokenInfo>(tokenId);
                if (token != null)
                {
                    token.IsRevoked = true;
                    await _cache.SetAsync(
                        tokenId,
                        token,
                        TimeSpan.FromDays(_jwtSettings.RefreshTokenExpiryDays)
                    );
                }
            }
        }
    }

    public Task RevokeUserTokensAsync(Guid userId)
    {
        return RevokeAllTokensAsync(userId);
    }

    public async Task StoreRefreshTokenAsync(Guid userId, string refreshToken)
    {
        var tokenInfo = new RefreshTokenInfo
        {
            UserId = userId,
            RefreshToken = refreshToken,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays),
            IsRevoked = false,
        };

        // Store the token with expiration
        await _cache.SetAsync(
            refreshToken,
            tokenInfo,
            TimeSpan.FromDays(_jwtSettings.RefreshTokenExpiryDays)
        );

        // Maintain a list of tokens per user (to enable revoking all tokens for a user)
        string userTokensKey = _keyProvider.GenerateKey("UserTokens", userId.ToString());

        // Get or create the list of user tokens
        var userTokens = await _cache.GetOrCreateAsync(
            userTokensKey,
            async () => new List<string>(),
            TimeSpan.FromDays(_jwtSettings.RefreshTokenExpiryDays * 2)
        ); // Double the expiry to ensure it outlives tokens

        // Add the new token to the list if not already present
        if (!userTokens.Contains(refreshToken))
        {
            userTokens.Add(refreshToken);

            // Update the token list in cache
            await _cache.SetAsync(
                userTokensKey,
                userTokens,
                TimeSpan.FromDays(_jwtSettings.RefreshTokenExpiryDays * 2)
            );
        }
    }
}
