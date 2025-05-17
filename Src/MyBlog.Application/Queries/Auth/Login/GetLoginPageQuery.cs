using MediatR;
using MyBlog.Core.Models;

namespace MyBlog.Application.Queries.Auth.Login;

public record GetLoginPageQuery(
    string ClientId,
    string RedirectUri,
    string[] Scopes,
    string CodeChallenge,
    string ChallengeMethod,
    string SessionId
) : IRequest<Result<bool>>;
