using MediatR;
using MyBlog.Core.Models;

namespace MyBlog.Application.Commands.Auth.Login;

public record LoginCommand(string UsernameOrEmail, string Password)
    : IRequest<Result<LoginResponse>>;
