using MediatR;

using MyBlog.Application.Models;
using MyBlog.Core.Aggregates.Blogs;
using MyBlog.Core.Models;
using MyBlog.Core.Models.Blogs;

namespace MyBlog.Application.Queries.Blogs.GetBlogs;

public record GetBlogsQuery(
    string? Title = null,
    BlogStatus? Status = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<Result<PaginationResult<BlogResponse>>> { }
