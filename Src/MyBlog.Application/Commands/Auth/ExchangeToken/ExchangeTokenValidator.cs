using FluentValidation;

namespace MyBlog.Application.Commands.Auth.ExchangeToken;

public class ExchangeTokenValidator : AbstractValidator<ExchangeTokenCommand>
{
    public ExchangeTokenValidator()
    {
        RuleFor(c => c.AuthCode).NotEmpty().WithMessage("Authorization code can't be empty");
        RuleFor(c => c.VerifierCode).NotEmpty().WithMessage("Verifier code can't be empty");
    }
}
