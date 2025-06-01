using MyBlog.Core.Aggregates.Users;

namespace MyBlog.Core.Models.Blogs;

public record BlogResponse(
    Guid Id,
    string Title,
    string Content,
    string AuthorName,
    string CategoryName,
    DateTime CreatedAt,
    DateTime? PublishDate,
    long ViewCount,
    IEnumerable<Comment>? Comments = null
)
{
    public static BlogResponse FromAggregate(
        Core.Aggregates.Blogs.BlogAggregate blog,
        string authorName,
        string categoryName,
        IDictionary<UserId, string>? userNames = null
    )
    {
        var comments = new List<Comment>();
        if (blog.Comments != null)
        {
            foreach (var comment in blog.Comments)
            {
                comments.Add(
                    Comment.FromEntity(
                        comment,
                        userNames?[comment.AuthorId] ?? "Unknown",
                        userNames
                    )
                );
            }
        }
        return new BlogResponse(
            blog.Id,
            blog.Title,
            blog.Content,
            authorName,
            categoryName,
            blog.CreatedAt,
            blog.PublishDate,
            blog.ViewCount,
            comments
        );
    }
}

public record Comment(
    Guid Id,
    string Content,
    string AuthorName,
    DateTime CreatedAt,
    IEnumerable<Comment>? ChildrenComments = null
)
{
    public static Comment FromEntity(
        Aggregates.Blogs.Comment comment,
        string authorName,
        IDictionary<UserId, string>? userNames = null
    )
    {
        var childrenComments = new List<Comment>();
        if (comment.ChildrenComment.Any())
        {
            foreach (var childComment in comment.ChildrenComment)
            {
                childrenComments.Add(
                    FromEntity(
                        childComment,
                        userNames?[childComment.AuthorId] ?? "Unknown",
                        userNames
                    )
                );
            }
        }
        return new Comment(
            comment.Id,
            comment.Content,
            authorName,
            comment.CreatedAt,
            childrenComments
        );
    }
}
