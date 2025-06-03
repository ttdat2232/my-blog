using MediatR;
using MyBlog.Core.Aggregates.Blogs;
using MyBlog.Core.Models;
using MyBlog.Core.Models.Blogs;
using MyBlog.Core.Repositories;
using MyBlog.Core.Services.Blogs;

namespace MyBlog.Application.Queries.Blogs.GetBlogBySlug;

public class GetBlogBySlugQueryHandler(IBlogService _blogService)
    : IRequestHandler<GetBlogBySlugQuery, Result<BlogResponse>>
{
    public Task<Result<BlogResponse>> Handle(
        GetBlogBySlugQuery request,
        CancellationToken cancellationToken
    )
    {
        return _blogService.GetBlogBySlugAsync(request.Slug, cancellationToken);
    }
}
