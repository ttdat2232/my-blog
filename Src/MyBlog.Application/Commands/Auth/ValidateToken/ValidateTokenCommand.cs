using MediatR;
using MyBlog.Core.Models;
using MyBlog.Core.Services.Auth.Models;

namespace MyBlog.Application.Commands.Auth.ValidateToken;

public record ValidateTokenCommand(string AccessToken) : IRequest<Result<TokenValidationResponse>>;
