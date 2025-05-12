using MediatR;
using MyBlog.Core.Models;

namespace MyBlog.Application.Queries.Blogs.GetBlogById;

public record GetBlogByIdQuery(Guid Id) : IRequest<Result<BlogResponse>>;
