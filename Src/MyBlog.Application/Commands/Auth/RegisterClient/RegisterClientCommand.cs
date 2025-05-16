using MediatR;
using MyBlog.Core.Models;

namespace MyBlog.Application.Commands.Auth.RegisterClient;

public record RegisterClientCommand(List<string> RedirectUris, List<string> AllowScopes)
    : IRequest<IResult>;
