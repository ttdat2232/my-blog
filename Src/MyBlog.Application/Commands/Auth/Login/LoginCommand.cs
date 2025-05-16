using MediatR;
using MyBlog.Core.Models;

namespace MyBlog.Application.Commands.Auth.Login;

public record LoginCommand(
    string UsernameOrEmail,
    string Password,
    string ClientId,
    string RedirectUri,
    string[] Scopes,
    string CodeChallenge,
    string ChallengeMethod
) : IRequest<Result<LoginResponse>>;
