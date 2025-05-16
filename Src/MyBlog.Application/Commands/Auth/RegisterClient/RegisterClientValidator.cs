using System;
using FluentValidation;

namespace MyBlog.Application.Commands.Auth.RegisterClient;

public class RegisterClientValidator : AbstractValidator<RegisterClientCommand>
{
    public RegisterClientValidator()
    {
        RuleFor(x => x.RedirectUris)
            .NotEmpty()
            .WithMessage("At least one redirect URI is required")
            .Must(uris => uris.All(uri => Uri.TryCreate(uri, UriKind.Absolute, out _)))
            .WithMessage("All redirect URIs must be valid absolute URIs");

        RuleForEach(x => x.RedirectUris)
            .NotEmpty()
            .WithMessage("Redirect URI cannot be empty")
            .MaximumLength(2000)
            .WithMessage("Redirect URI cannot exceed 2000 characters");

        RuleFor(x => x.AllowScopes).NotEmpty().WithMessage("At least one scope is required");

        RuleForEach(x => x.AllowScopes)
            .NotEmpty()
            .WithMessage("Scope cannot be empty")
            .MaximumLength(100)
            .WithMessage("Scope cannot exceed 100 characters")
            .Matches("^[a-zA-Z0-9._-]+$")
            .WithMessage(
                "Scope can only contain alphanumeric characters, dots, underscores, and hyphens"
            );
    }
}
