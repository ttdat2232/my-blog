using MediatR;
using MyBlog.Core.Aggregates.Blogs;
using MyBlog.Core.Models;

namespace MyBlog.Application.Commands.Blogs.UpdateBlog;

public record UpdateBlogCommand(
    Guid Id,
    string Title,
    string Content,
    BlogStatus Status,
    Guid RequestUpdateUserId,
    Guid? CategoryId = null
) : IRequest<Result<bool>>;
