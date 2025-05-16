using Microsoft.AspNetCore.Mvc;

namespace MyBlog.Auth.Controllers;

[Route("[controller]")]
public class AuthController : ControllerBase
{
    [HttpGet("login")]
    public async Task<IActionResult> GetLoginPage()
    {
        using var stream = new StreamReader("Templates/Login.html");

        return new ContentResult()
        {
            ContentType = "text/html",
            Content = await stream.ReadToEndAsync(),
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
}
