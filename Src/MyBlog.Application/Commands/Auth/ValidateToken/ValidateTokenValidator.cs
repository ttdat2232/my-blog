using FluentValidation;

namespace MyBlog.Application.Commands.Auth.ValidateToken;

public class ValidateTokenValidator : AbstractValidator<ValidateTokenCommand>
{
    public ValidateTokenValidator()
    {
        RuleFor(request => request.AccessToken).NotEmpty().WithMessage("Access token is required");
    }
}
