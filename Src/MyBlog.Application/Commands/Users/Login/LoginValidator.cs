using FluentValidation;

namespace MyBlog.Application.Commands.Users.Login;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.UsernameOrEmail).NotEmpty().MinimumLength(3).MaximumLength(100);

        RuleFor(x => x.Password).NotEmpty().MinimumLength(6).MaximumLength(100);
    }
}
