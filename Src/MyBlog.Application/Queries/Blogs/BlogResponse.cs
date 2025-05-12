namespace MyBlog.Application.Queries.Blogs;

public record BlogResponse(
    Guid Id,
    string Title,
    string Content,
    string AuthorName,
    string CategoryName,
    DateTime CreatedAt,
    DateTime? PublishDate,
    long ViewCount
);
