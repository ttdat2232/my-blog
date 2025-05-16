using FluentValidation;

namespace MyBlog.Application.Commands.Auth.RefreshToken;

public class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("Refresh token is required");

        RuleFor(x => x.AccessToken).NotEmpty().WithMessage("Access token is required");
    }
}
