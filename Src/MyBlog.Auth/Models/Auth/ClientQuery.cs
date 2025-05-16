namespace MyBlog.Auth.Models.Auth;

public record ClientQuery(
    string ClientId,
    string RedirectUri,
    string[] Scopes,
    string CodeChallenge,
    string ChallengeMethod
);
