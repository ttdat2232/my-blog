using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using MyBlog.Core.Aggregates.Roles;
using MyBlog.Core.Aggregates.Users;
using MyBlog.Core.Repositories;
using MyBlog.Core.Services.Auth.Tokens;

namespace MyBlog.WebApi.Middlewares;

public class MyBlogAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public MyBlogAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        ITokenService tokenService,
        IUnitOfWork unitOfWork,
        UrlEncoder encoder
    )
        : base(options, logger, encoder)
    {
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
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
        if (!validateTokenData.Roles.Any())
        {
            var users = await _unitOfWork
                .Repository<UserAggregate, UserId>()
                .GetAsync(
                    expression: u => u.Id == UserId.From(validateTokenData.UserId),
                    includeStrings: ["_roles"]
                );
            if (users.Any())
            {
                var user = users.First();
                if (user.Roles != null && user.Roles.Any())
                {
                    var roleIds = user.Roles.Select(r => r.RoleId).ToList();
                    var roles = await _unitOfWork
                        .Repository<RoleAggregate, RoleId>()
                        .GetAsync(
                            expression: r => roleIds.Contains(r.Id),
                            select: r => r.NormalizeName
                        );

                    foreach (var role in roles)
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, role));
                        identity.AddClaim(new Claim("role", role));
                    }
                }
            }
        }
        var principal = new ClaimsPrincipal(identity);
        return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
    }
}
