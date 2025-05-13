using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Application.Commands.Users.Login;
using MyBlog.Application.Commands.Users.Register;
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
