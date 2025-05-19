using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Application.Commands.Auth.Login;
using MyBlog.Auth.Extensions;
using MyBlog.Auth.Models.Auth;

namespace MyBlog.Auth.Controllers;

[Route("[controller]")]
public class AuthController(ISender sender) : Controller
{
    [HttpGet]
    [HttpGet("login")]
    public async Task<IActionResult> Index(
        [FromQuery] ClientQuery query,
        CancellationToken cancellationToken
    )
    {
        var loginPageQuery = new Application.Queries.Auth.Login.GetLoginPageQuery(
            query.ClientId,
            query.RedirectUri,
            query.Scopes,
            query.CodeChallenge,
            query.ChallengeMethod,
            HttpContext.Session.Id
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
        return View(new LoginRequest("", "", HttpContext.Session.Id));
    }

    [HttpPost]
    public async Task<IActionResult> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new LoginCommand(
            request.UsernameOrEmail,
            request.Password,
            request.SessionId
        );
        var result = await sender.Send(command, cancellationToken);
        if (result.IsFailure)
            return View("index");
        return Redirect(
            $"{result.Data.RedirectUri}?authorizationCode={result.Data.AuthorizationCode}"
        );
    }
}
