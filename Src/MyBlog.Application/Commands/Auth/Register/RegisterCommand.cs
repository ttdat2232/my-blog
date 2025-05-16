using MediatR;
using MyBlog.Core.Models;

namespace MyBlog.Application.Commands.Auth.Register;

public record RegisterCommand(string Username, string Email, string Password)
    : IRequest<Result<RegisterResponse>>;
