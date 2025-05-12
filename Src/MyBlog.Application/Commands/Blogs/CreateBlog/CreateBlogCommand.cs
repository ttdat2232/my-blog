using MediatR;
using MyBlog.Core.Models;

namespace MyBlog.Application.Commands.Blogs.CreateBlog;

public record CreateBlogCommand(
    string Title,
    string Content,
    Guid AuthorId,
    Guid CategoryId,
    DateTime? PublishDate,
    bool IsDraft = false
) : IRequest<Result<Guid>>;
