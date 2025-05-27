using System.Security.Claims;

namespace MyBlog.WebApi.Extensions;

public static class AuthenticatedUserExtension
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var userId =
            user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? user.FindFirstValue("sub")
            ?? user.FindFirstValue("userId");
        return Guid.TryParse(userId, out var id) ? id : Guid.Empty;
    }
}
