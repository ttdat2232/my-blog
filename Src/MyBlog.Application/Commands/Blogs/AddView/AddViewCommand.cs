using MediatR;
using MyBlog.Core.Models;

namespace MyBlog.Application.Commands.Blogs.AddView;

public record AddViewCommand(Guid blogId) : IRequest<Result<bool>>;
