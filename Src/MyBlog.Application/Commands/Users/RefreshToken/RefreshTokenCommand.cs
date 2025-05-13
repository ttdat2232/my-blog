using MediatR;
using MyBlog.Core.Models;

namespace MyBlog.Application.Commands.Users.RefreshToken;

public record RefreshTokenCommand(string AccessToken, string RefreshToken)
    : IRequest<Result<RefreshTokenResponse>>;
