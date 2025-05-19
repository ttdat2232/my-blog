using MediatR;
using MyBlog.Core.Aggregates.Users;
using MyBlog.Core.Models;
using MyBlog.Core.Models.Auth;
using MyBlog.Core.Repositories;
using MyBlog.Core.Services.Auth.Tokens;
using MyBlog.Core.Services.Cache;

namespace MyBlog.Application.Commands.Auth.ExchangeToken;

public class ExchangeTokenCommandHandler(
    ITokenService _tokenService,
    ICacheService _cacheService,
    IUnitOfWork _unitOfWork
) : IRequestHandler<ExchangeTokenCommand, Result<ExchangeTokenResponse>>
{
    public async Task<Result<ExchangeTokenResponse>> Handle(
        ExchangeTokenCommand request,
        CancellationToken cancellationToken
    )
    {
        var authCodeChallengeInfo =
            await _cacheService.GetAndRemoveAsync<AuthCodeChallengeInformation>(request.AuthCode);
        if (authCodeChallengeInfo == null)
            return Result<ExchangeTokenResponse>.Failure(UserErrors.InvalidAuthorizationCode);

        if (
            !IsValidCodeVerifier(
                request.VerifierCode,
                authCodeChallengeInfo.CodeChallenge,
                authCodeChallengeInfo.CodeChallengeMethod
            )
        )
            return Result<ExchangeTokenResponse>.Failure(UserErrors.InvalidCodeVerifier);

        var user = (
            await _unitOfWork
                .Repository<UserAggregate, UserId>()
                .FindById(UserId.From(authCodeChallengeInfo.UserId!.Value), cancellationToken)
        )!;
        var generateTokenResult = await _tokenService.GenerateTokenAsync(
            user.Id,
            user.UserName,
            user.Email
        );
        if (generateTokenResult.IsFailure)
            return Result<ExchangeTokenResponse>.Failure(
                new("Server failed when generating token", 500)
            );
        var tokenData = generateTokenResult.Data;
        return Result<ExchangeTokenResponse>.Success(
            new(tokenData.AccessToken, tokenData.RefreshToken, tokenData.ExpiresIn)
        );
    }

    private static bool IsValidCodeVerifier(
        string codeVerifier,
        string codeChallenge,
        string codeChallengeMethod = "S256"
    )
    {
        if (string.IsNullOrEmpty(codeVerifier) || string.IsNullOrEmpty(codeChallenge))
            return false;

        if (codeChallengeMethod == "S256")
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.ASCII.GetBytes(codeVerifier);
            var hash = sha256.ComputeHash(bytes);
            var transformed = Base64UrlEncode(hash);
            return transformed == codeChallenge;
        }
        else if (codeChallengeMethod == "plain")
        {
            return codeVerifier == codeChallenge;
        }
        else
        {
            return false;
        }
    }

    private static string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }
}
