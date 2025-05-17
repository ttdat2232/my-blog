using MyBlog.Core.Aggregates.Users;

namespace MyBlog.Core.Models.Auth;

public class AuthCodeChallengeInformation
{
    public string CodeChallenge { get; private set; } = null!;
    public string CodeChallengeMethod { get; private set; } = null!;
    public string ClientId { get; private set; }
    public string RedirectUri { get; private set; }
    public string[] Scopes { get; private set; }
    public string? AuthorizationCode { get; private set; }
    public Guid? UserId { get; private set; }

    private AuthCodeChallengeInformation() { }

    public static AuthCodeChallengeInformation Create(
        string codeChallenge,
        string codeChallengeMethod,
        string clientId,
        string redirectUri,
        string[] scopes
    ) =>
        new()
        {
            CodeChallenge = codeChallenge,
            CodeChallengeMethod = codeChallengeMethod,
            ClientId = clientId,
            RedirectUri = redirectUri,
            Scopes = scopes,
        };

    public void AddUserInformation(UserId userId) => UserId = userId;

    public void AddAuthorizationCode(string authorizationCode) =>
        AuthorizationCode = authorizationCode;
}
