using MyBlog.Core.Models;
using MyBlog.Core.Services.Auth.Models;

namespace MyBlog.Core.Services.Auth;

public interface IAuthService
{
    Task<Result<TokenResponse>> GenerateTokenAsync(Guid userId, string username, string email);
    Task<Result<TokenResponse>> RefreshTokenAsync(string refreshToken);
    Task<bool> ValidateTokenAsync(string token);
    Task<Result<TokenValidationResponse>> ValidateAndDecodeTokenAsync(string token);
    Task<Result<bool>> RevokeTokenAsync(Guid userId);
    Task<Result<bool>> RevokeAllTokensAsync(Guid userId);
}
