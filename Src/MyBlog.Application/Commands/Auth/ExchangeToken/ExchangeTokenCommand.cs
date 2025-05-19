using MediatR;
using MyBlog.Core.Models;

namespace MyBlog.Application.Commands.Auth.ExchangeToken;

public record ExchangeTokenCommand(string AuthCode, string VerifierCode)
    : IRequest<Result<ExchangeTokenResponse>>;
