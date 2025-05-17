using MediatR;
using MyBlog.Core.Aggregates.Clients;
using MyBlog.Core.Aggregates.Users;
using MyBlog.Core.Models;
using MyBlog.Core.Models.Auth;
using MyBlog.Core.Repositories;
using MyBlog.Core.Services.Cache;

namespace MyBlog.Application.Commands.Auth.Login;

public class LoginCommandHandler(IUnitOfWork _unitOfWork, ICacheService _cacheService)
    : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken
    )
    {
        var userRepo = _unitOfWork.Repository<UserAggregate, UserId>();
        var user = await userRepo.FindBy(u =>
            u.NormalizeUserName.Equals(request.UsernameOrEmail.ToLower())
            || u.NormalizeEmail.Equals(request.UsernameOrEmail.ToLower())
        );

        if (user == null)
            return Result<LoginResponse>.Failure(UserErrors.InvalidCredentials);

        // if (!user.ValidatePassword(request.Password))
        //     return Result<LoginResponse>.Failure(UserErrors.InvalidCredentials);

        var authorizaeCodeInfo =
            await _cacheService.GetAndRemoveAsync<AuthCodeChallengeInformation>(
                request.SessionId,
                cancellationToken
            );
        if (authorizaeCodeInfo == null)
            return Result<LoginResponse>.Failure(UserErrors.InvalidCredentials);

        var authorizationCode = GenerateAuthorizationCode();
        authorizaeCodeInfo.AddAuthorizationCode(authorizationCode);
        _ = _cacheService
            .SetAsync(authorizationCode, authorizaeCodeInfo)
            .ContinueWith(t =>
            {
                if (t.Status == TaskStatus.RanToCompletion)
                    Serilog.Log.Information("Set authorization code to redis complete");
            });

        return Result<LoginResponse>.Success(
            new(authorizationCode, authorizaeCodeInfo.RedirectUri)
        );
    }

    private static string GenerateAuthorizationCode()
    {
        return Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
    }
}
