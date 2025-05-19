using MyBlog.Core.Models;

namespace MyBlog.Core.Aggregates.Users;

public static class UserErrors
{
    public static readonly Error UsernameExisted = new("Username was already taken", 400);
    public static readonly Error EmailExisted = new("Email was already taken", 400);
    public static readonly Error InvalidCredentials = new("Username or passowrd is wrong", 401);
    public static readonly Error InvalidCodeVerifier = new("Code Verifier is wrong", 401);
    public static readonly Error InvalidAuthorizationCode = new("Authorization code is wrong", 401);
    public static readonly Error InvalidClient = new("Invalid client", 401);
    public static readonly Error RegisterFailed = new("Register failed", 500);
}
