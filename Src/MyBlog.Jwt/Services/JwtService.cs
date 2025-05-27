using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyBlog.Core.Models;
using MyBlog.Core.Repositories;
using MyBlog.Core.Services.Auth.Models;
using MyBlog.Core.Services.Auth.Tokens;
using MyBlog.Jwt.Configurations;
using Serilog;

namespace MyBlog.Jwt.Services;

public class JwtService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly ITokenRepository _tokenRepository;
    private readonly IUnitOfWork _unitOfWork;

    public JwtService(
        IOptions<JwtSettings> jwtSettings,
        ITokenRepository tokenRepository,
        IUnitOfWork unitOfWork
    )
    {
        _jwtSettings = jwtSettings.Value;
        _tokenRepository = tokenRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TokenResponse>> GenerateTokenAsync(
        Guid userId,
        string username,
        string email
    )
    {
        try
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new(JwtRegisteredClaimNames.Email, email),
                new("username", username),
                new("userId", userId.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            var roleResult = await _unitOfWork.RoleRepository.GetRolesAsync(userId);
            if (roleResult.IsSuccess)
            {
                foreach (var role in roleResult.Data)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                    claims.Add(new Claim("role", role));
                }
            }
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                signingCredentials: signingCredentials
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = GenerateRefreshToken();

            await _tokenRepository.StoreRefreshTokenAsync(userId, refreshToken);

            return Result<TokenResponse>.Success(
                new TokenResponse(accessToken, refreshToken, _jwtSettings.ExpiryMinutes * 60)
            );
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error when generating token: {Message}", ex.Message);
            return Result<TokenResponse>.Failure(new Error("Error when generate token", 500));
        }
    }

    public async Task<Result<TokenResponse>> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            var storedToken = await _tokenRepository.GetRefreshTokenAsync(refreshToken);
            if (storedToken == null)
                return Result<TokenResponse>.Failure(new Error("Invalid refresh token", 400));

            return await GenerateTokenAsync(
                storedToken.UserId,
                storedToken.Username,
                storedToken.Email
            );
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error when refreshing token: {Message}", ex.Message);
            return Result<TokenResponse>.Failure(new Error("Failed to refresh token", 500));
        }
    }

    public async Task<bool> ValidateTokenAsync(
        string token,
        Action<ValidationOption>? config = null
    )
    {
        var result = await ValidateAndDecodeTokenAsync(token, config);
        return result.IsSuccess;
    }

    public async Task<Result<TokenValidationResponse>> ValidateAndDecodeTokenAsync(
        string token,
        Action<ValidationOption>? config = null
    )
    {
        var opts = ValidationOption.Default;
        if (config != null)
            config(opts);

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = opts.IsValidateIssuer,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = opts.IsValidateAudience,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = opts.IsValidateLifeTime,
                ValidateSignatureLast = opts.IsValidateSignature,
                ClockSkew = TimeSpan.Zero,
            };

            var principal = tokenHandler.ValidateToken(
                token,
                validationParameters,
                out var validatedToken
            );
            var jwtToken = (JwtSecurityToken)validatedToken;

            var claims = new Dictionary<string, string>();
            foreach (var claim in jwtToken.Claims)
            {
                claims[claim.Type] = claim.Value;
            }
            var response = new TokenValidationResponse(
                principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                    ?? principal.FindFirst("userId")?.Value
                    ?? "",
                principal.FindFirst(ClaimTypes.Name)?.Value!,
                principal.FindFirst(JwtRegisteredClaimNames.Email)?.Value!,
                principal
                    .Claims.Where(x => x.Type == ClaimTypes.Role)
                    .Select(x => x.Value)
                    .ToList(),
                claims,
                jwtToken.ValidTo
            );

            return Result<TokenValidationResponse>.Success(response);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error when validating token: {Message}", ex.Message);
            return Result<TokenValidationResponse>.Failure(
                new Error("Token validation failed", 500)
            );
        }
    }

    public async Task<Result<bool>> RevokeTokenAsync(Guid userId)
    {
        try
        {
            await _tokenRepository.RevokeUserTokensAsync(userId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error when revoking token", 500);
            return Result<bool>.Failure(new Error("Failed to revoke token", 500));
        }
    }

    public async Task<Result<bool>> RevokeAllTokensAsync(Guid userId)
    {
        try
        {
            await _tokenRepository.RevokeAllTokensAsync(userId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error when revoking all tokens: {Message}", ex.Message);
            return Result<bool>.Failure(new Error("Failed to revoke all tokens", 500));
        }
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
