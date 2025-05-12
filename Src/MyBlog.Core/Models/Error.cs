namespace MyBlog.Core.Models;

public record Error(string Description, int Code)
{
    public override string ToString()
    {
        return $"Error: {Description}";
    }

    public static readonly Error None = new(string.Empty, 0);
}
