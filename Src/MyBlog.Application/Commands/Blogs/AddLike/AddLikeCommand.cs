using MediatR;
using MyBlog.Core.Models;

namespace MyBlog.Application.Commands.Blogs.AddLike;

public record AddLikeCommand(Guid UserId, Guid BlogId) : IRequest<Result<bool>>;
