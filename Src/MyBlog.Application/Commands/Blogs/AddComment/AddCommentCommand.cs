using MediatR;
using MyBlog.Core.Models;

namespace MyBlog.Application.Commands.Blogs.AddComment;

public record AddCommentCommand(
    Guid BlogId,
    string Content,
    Guid AuthorId,
    Guid? ParentCommentId = null
) : IRequest<Result<bool>>
{
    public static AddCommentCommand Create(
        Guid blogId,
        string content,
        Guid authorId,
        Guid? parentCommentId = null
    )
    {
        return new AddCommentCommand(blogId, content, authorId, parentCommentId);
    }
}
