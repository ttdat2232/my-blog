using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using MyBlog.Core.Services.Auth.Tokens;

namespace MyBlog.WebApi.Middlewares;

public class MyBlogAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly ITokenService _tokenService;

    public MyBlogAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        ITokenService tokenService,
        UrlEncoder encoder
    )
        : base(options, logger, encoder)
    {
        _tokenService = tokenService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authHeader = Request.Headers.Authorization.ToString();
        if (string.IsNullOrEmpty(authHeader))
            return AuthenticateResult.NoResult();

        var token = authHeader.Replace("Bearer ", "");
        var validateTokenResult = await _tokenService.ValidateAndDecodeTokenAsync(
            token,
            config =>
            {
                config.IsValidateSignature = false;
#if DEBUG
                config.IsValidateLifeTime = false;
#endif
            }
        );
        if (validateTokenResult.IsFailure)
            return AuthenticateResult.Fail(validateTokenResult.Error.Description);

        var validateTokenData = validateTokenResult.Data;
        var identity = new ClaimsIdentity(
            validateTokenData.Claims.Select(c => new Claim(c.Key, c.Value)),
            Scheme.Name
        );
        var principal = new ClaimsPrincipal(identity);
        return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
    }
}
