namespace MyBlog.Core.Models;

public record Error(string Description, int Code)
{
    public static readonly Error None = new(string.Empty, 0);
}
