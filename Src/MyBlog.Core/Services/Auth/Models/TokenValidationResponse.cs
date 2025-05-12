namespace MyBlog.Core.Services.Auth.Models;

public record TokenValidationResponse(
    string UserId,
    string Username,
    string Email,
    List<string> Roles,
    Dictionary<string, string> Claims,
    DateTime ExpirationTime);