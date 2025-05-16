using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Application.Commands.Auth.Login;
using MyBlog.Auth.Extensions;
using MyBlog.Auth.Models.Auth;

namespace MyBlog.Auth.Controllers;

[Route("[controller]")]
public class AuthController(ISender sender) : ControllerBase
{
    [HttpGet]
    [HttpGet("login")]
    public async Task<IActionResult> GetLoginPage(
        [FromQuery] ClientQuery query,
        CancellationToken cancellationToken
    )
    {
        var loginPageQuery = new Application.Queries.Auth.Login.GetLoginPageQuery(
            query.ClientId,
            query.RedirectUri,
            query.Scopes,
            query.CodeChallenge,
            query.ChallengeMethod
        );
        var result = await sender.Send(loginPageQuery, cancellationToken);
        if (!result.IsSuccess)
        {
            using var stream = new StreamReader("Templates/Forbiden.html");
            return new ContentResult()
            {
                ContentType = "text/html",
                Content = await stream.ReadToEndAsync(),
                StatusCode = 403,
            };
        }
        using var loginStream = new StreamReader("Templates/Login.html");
        return new ContentResult()
        {
            ContentType = "text/html",
            Content = await loginStream.ReadToEndAsync(),
        };
    }

    [HttpGet("register")]
    [HttpGet("sign-in")]
    public async Task<IActionResult> GetRegisterPage()
    {
        using var stream = new StreamReader("Templates/Register.html");

        return new ContentResult()
        {
            ContentType = "text/html",
            Content = await stream.ReadToEndAsync(),
        };
    }

    #region  API

    [HttpPost("~/api/[controller]")]
    public async Task<IActionResult> LoginAsync(
        [FromQuery] ClientQuery query,
        [FromBody] Models.Auth.LoginRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new LoginCommand(
            request.UsernameOrEmail,
            request.Password,
            query.ClientId,
            query.RedirectUri,
            query.Scopes,
            query.CodeChallenge,
            query.ChallengeMethod
        );
        var result = await sender.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    #endregion
}
