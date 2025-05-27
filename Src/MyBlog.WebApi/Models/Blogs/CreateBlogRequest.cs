namespace MyBlog.WebApi.Models.Blogs;

public record CreateBlogRequest(
    string Title,
    string Content,
    int Status,
    Guid CategoryId,
    DateTime PublishDate,
    bool IsDraft
);
