using MediatR;
using MyBlog.Core.Aggregates.Users;
using MyBlog.Core.Models;
using MyBlog.Core.Repositories;
using MyBlog.Core.Services.Auth;

namespace MyBlog.Application.Commands.Users.Login;

public class LoginCommandHandler(IUnitOfWork _unitOfWork, IAuthService _authService)
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

        if (!user.ValidatePassword(request.Password))
            return Result<LoginResponse>.Failure(UserErrors.InvalidCredentials);

        var result = await _authService.GenerateTokenAsync(
            user.Id.Value,
            user.UserName,
            user.Email
        );

        if (result.IsFailure)
            return Result<LoginResponse>.Failure(result.Error);
        return Result<LoginResponse>.Success(new(result.Data.AccessToken, result.Data.ExpiresIn));
    }
}
