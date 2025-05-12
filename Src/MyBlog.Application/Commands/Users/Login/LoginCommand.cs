using MediatR;
using MyBlog.Core.Models;

namespace MyBlog.Application.Commands.Users.Login;

public record LoginCommand(string UsernameOrEmail, string Password)
    : IRequest<Result<LoginResponse>>;
