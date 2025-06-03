using MediatR;
using MyBlog.Core.Aggregates.Roles;
using MyBlog.Core.Aggregates.Users;
using MyBlog.Core.Models;
using MyBlog.Core.Repositories;
using MyBlog.Core.Services.Auth.Tokens;
using MyBlog.Core.Services.Cache;

namespace MyBlog.Application.Commands.Auth.Register;

public class RegisterCommandHandler(
    IUnitOfWork _unitOfWork,
    ITokenService _authService,
    ICacheService _cacheService
) : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
{
    public async Task<Result<RegisterResponse>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken
    )
    {
        var userRepo = _unitOfWork.Repository<UserAggregate, UserId>();
        if (await userRepo.IsExisted(u => u.NormalizeUserName.Equals(request.Username.ToLower())))
            return Result<RegisterResponse>.Failure(UserErrors.UsernameExisted);

        if (await userRepo.IsExisted(u => u.NormalizeEmail.Equals(request.Email.ToLower())))
            return Result<RegisterResponse>.Failure(UserErrors.EmailExisted);

        var user = UserAggregate.Create(request.Username, request.Email, request.Password);
        await userRepo.AddAsync(user);
        var userRole = await _cacheService.GetOrCreateAsync(
            ["role", "user"],
            async () =>
            {
                var roleRepo = _unitOfWork.Repository<RoleAggregate, RoleId>();
                var role = (
                    await roleRepo.GetAllAsync(
                        r => r.NormalizeName.Equals("user"),
                        cancellationToken
                    )
                ).FirstOrDefault();
                if (role is null)
                    throw new InvalidOperationException("User role not found");
                return role;
            },
            TimeSpan.FromDays(30),
            cancellationToken
        );
        user.AddRole(userRole);

        if (!await _unitOfWork.SaveAsync(cancellationToken))
            return Result<RegisterResponse>.Failure(UserErrors.RegisterFailed);
        var tokenResult = await _authService.GenerateTokenAsync(user.Id, user.UserName, user.Email);
        if (tokenResult.IsFailure)
            return Result<RegisterResponse>.Failure(tokenResult.Error);
        return Result<RegisterResponse>.Success(
            new(tokenResult.Data.AccessToken, tokenResult.Data.ExpiresIn)
        );
    }
}
