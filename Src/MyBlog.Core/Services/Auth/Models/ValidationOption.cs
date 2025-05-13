namespace MyBlog.Core.Services.Auth.Models;

public class ValidationOption
{
    public bool IsValidateLifeTime { get; set; }

    public static ValidationOption Default => new() { IsValidateLifeTime = true };
}
