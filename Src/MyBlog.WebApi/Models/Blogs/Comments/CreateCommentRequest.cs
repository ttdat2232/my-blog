namespace MyBlog.WebApi.Models.Blogs.Comments;

public record CreateCommentRequest(string Content, Guid? ParentCommentId = null);
