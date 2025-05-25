namespace MyBlog.Core.Services.Auth.Models;

public class ValidationOption
{
    public bool IsValidateLifeTime { get; set; }
    public bool IsValidateAudience { get; set; }
    public bool IsValidateIssuer { get; set; }
    public bool IsValidateSignature { get; set; }

    public static ValidationOption Default =>
        new()
        {
            IsValidateLifeTime = true,
            IsValidateAudience = true,
            IsValidateIssuer = true,
            IsValidateSignature = true,
        };
}
