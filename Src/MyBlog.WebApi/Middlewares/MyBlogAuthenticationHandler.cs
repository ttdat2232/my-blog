using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace MyBlog.WebApi.Middlewares;

public class MyBlogAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IHttpClientFactory _clientFactory;

    public MyBlogAuthenticationHandler(
        IHttpClientFactory clientFactory,
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder
    )
        : base(options, logger, encoder)
    {
        _clientFactory = clientFactory;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authHeader = Request.Headers.Authorization.ToString();
        if (!string.IsNullOrEmpty(authHeader))
            return AuthenticateResult.NoResult();

        var token = authHeader.Replace("Bearer", "");
        var client = _clientFactory.CreateClient("auth");
        var requestBody = new Dictionary<string, object>
        {
            { "token", token },
            { "clientId", "test" },
            { "clientSecret", "test" },
        };
        var jsonBody = JsonSerializer.Serialize(requestBody);
        var response = await client.PostAsync(
            "/validate-access-token",
            new StringContent(jsonBody, Encoding.UTF8, "application/json")
        );
        if (!response.IsSuccessStatusCode)
            return AuthenticateResult.Fail("Invalid token");

        var claims = await ParseClaimsFromResponse(response);
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
    }

    private static async Task<List<Claim>> ParseClaimsFromResponse(HttpResponseMessage response)
    {
        // Example: Deserialize JSON response to extract claims
        var content = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        if (content == null)
            return new();
        var claims = new List<Claim>();
        if (content.TryGetValue("sub", out var sub))
            claims.Add(new Claim(ClaimTypes.NameIdentifier, sub.ToString()!));

        if (content.TryGetValue("email", out var email))
            claims.Add(new Claim(ClaimTypes.NameIdentifier, email.ToString()!));
        if (content.TryGetValue("userId", out var userId))
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userId.ToString()!));
        if (content.TryGetValue("username", out var username))
            claims.Add(new Claim(ClaimTypes.NameIdentifier, username.ToString()!));
        return claims;
    }
}
