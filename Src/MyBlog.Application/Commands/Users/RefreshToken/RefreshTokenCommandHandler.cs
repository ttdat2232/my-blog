using MediatR;
using MyBlog.Core.Models;
using MyBlog.Core.Services.Auth;

namespace MyBlog.Application.Commands.Users.RefreshToken;

public class RefreshTokenCommandHandler(IAuthService _authService)
    : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
{
    public async Task<Result<RefreshTokenResponse>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken
    )
    {
        var accessTokenValidationResult = await _authService.ValidateAndDecodeTokenAsync(
            request.AccessToken,
            opts => opts.IsValidateLifeTime = false
        );
        if (accessTokenValidationResult.IsFailure)
            return Result<RefreshTokenResponse>.Failure(accessTokenValidationResult.Error);

        var refreshResult = await _authService.RefreshTokenAsync(request.RefreshToken);

        if (refreshResult.IsFailure)
            return Result<RefreshTokenResponse>.Failure(refreshResult.Error);

        return Result<RefreshTokenResponse>.Success(
            new(
                refreshResult.Data.RefreshToken,
                refreshResult.Data.AccessToken,
                refreshResult.Data.ExpiresIn
            )
        );
    }
}
