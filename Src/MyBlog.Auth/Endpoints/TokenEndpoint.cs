using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Application.Commands.Auth.ExchangeToken;
using MyBlog.Auth.Extensions;

namespace MyBlog.Auth.Endpoints;

public class TokenEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/token");
        group.MapGet("", ExchangeTokenAsync);
    }

    private static async Task<IActionResult> ExchangeTokenAsync(
        [FromQuery] string authorizationCode,
        [FromQuery] string veriferCode,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var exchangeTokenCommand = new ExchangeTokenCommand(authorizationCode, veriferCode);
        var tokenResult = await sender.Send(exchangeTokenCommand, cancellationToken);
        return tokenResult.ToActionResult();
    }
}
