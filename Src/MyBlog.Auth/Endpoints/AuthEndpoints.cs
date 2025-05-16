using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Application.Commands.Auth.Login;
using MyBlog.Application.Commands.Auth.Register;
using MyBlog.Application.Commands.Auth.ValidateToken;
using MyBlog.Auth.Extensions;
using MyBlog.Auth.Models.Auth;

namespace MyBlog.Auth.Endpoints;

public class AuthEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/auth");
        group.MapPost("login", LoginAsync);
        group.MapPost("register", RegisterAsync);
        group.MapPost("validate-access-token", ValidateTokenAsync);
    }

    private static async Task<IActionResult> ValidateTokenAsync(
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var token = context.Request.Query["Token"].ToString();
        if (string.IsNullOrEmpty(token))
        {
            token = context.Request.Headers.Authorization.ToString().Replace("Bearer", "");
        }

        var validateTokenCommand = new ValidateTokenCommand(token);
        var result = await sender.Send(validateTokenCommand, cancellationToken);
        return result.ToActionResult();
    }

    private static async Task<IActionResult> RegisterAsync(
        RegisterRequest registerRequest,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var command = new RegisterCommand(
            registerRequest.Username,
            registerRequest.Email,
            registerRequest.Password
        );
        var result = await sender.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    private static async Task<IActionResult> LoginAsync(
        LoginRequest request,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var command = new LoginCommand(request.UsernameOrEmail, request.Password);
        var result = await sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }
}
