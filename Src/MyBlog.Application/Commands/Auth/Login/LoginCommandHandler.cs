using MediatR;
using MyBlog.Core.Aggregates.Clients;
using MyBlog.Core.Aggregates.Users;
using MyBlog.Core.Models;
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
        var clientRepo = _unitOfWork.Repository<ClientAggregate, ClientId>();
        Serilog.Log.Information("fetching data of client {Id}", request.ClientId);
        if (!Guid.TryParse(request.ClientId.Trim(), out var clientId))
            return Result<LoginResponse>.Failure(new("Invalidate Client Id", 401));

        var client = await clientRepo.FindById(ClientId.From(clientId), cancellationToken);
        if (client == null)
            return Result<LoginResponse>.Failure(new("Client not found", 404));

        if (!client.RedirectUris.Contains(request.RedirectUri))
            return Result<LoginResponse>.Failure(new("Invalid redirect uri", 400));

        if (request.Scopes.Any(scope => !client.AllowScopes.Contains(scope)))
            return Result<LoginResponse>.Failure(new("Invalid scope(s)", 400));

        if (
            string.IsNullOrWhiteSpace(request.CodeChallenge)
            || string.IsNullOrWhiteSpace(request.ChallengeMethod)
        )
            return Result<LoginResponse>.Failure(new("Invalid code challenge or method", 400));

        var userRepo = _unitOfWork.Repository<UserAggregate, UserId>();
        var user = await userRepo.FindBy(u =>
            u.NormalizeUserName.Equals(request.UsernameOrEmail.ToLower())
            || u.NormalizeEmail.Equals(request.UsernameOrEmail.ToLower())
        );

        if (user == null)
            return Result<LoginResponse>.Failure(UserErrors.InvalidCredentials);

        if (!user.ValidatePassword(request.Password))
            return Result<LoginResponse>.Failure(UserErrors.InvalidCredentials);

        var authorizationCode = GenerateAuthorizationCode();
        _ = _cacheService
            .SetAsync(authorizationCode, request.CodeChallenge)
            .ContinueWith(_ => Serilog.Log.Information("Set authorization code to redis complete"));

        return Result<LoginResponse>.Success(new(authorizationCode, request.RedirectUri));
    }

    private static string GenerateAuthorizationCode()
    {
        return Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
    }
}
