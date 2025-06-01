using MediatR;
using MyBlog.Core.Models;
using MyBlog.Core.Models.Blogs;

namespace MyBlog.Application.Queries.Blogs.GetBlogById;

public record GetBlogByIdQuery(Guid Id) : IRequest<Result<BlogResponse>>;
