using MediatR;
using MyBlog.Core.Models;
using MyBlog.Core.Services.Auth;
using MyBlog.Core.Services.Auth.Models;

namespace MyBlog.Application.Commands.Auth.ValidateToken;

public class ValidateTokenCommandHandler(ITokenService _authService)
    : IRequestHandler<ValidateTokenCommand, Result<TokenValidationResponse>>
{
    public Task<Result<TokenValidationResponse>> Handle(
        ValidateTokenCommand request,
        CancellationToken cancellationToken
    )
    {
        return _authService.ValidateAndDecodeTokenAsync(request.AccessToken);
    }
}
