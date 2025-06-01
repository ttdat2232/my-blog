using MediatR;
using MyBlog.Core.Aggregates.Blogs;
using MyBlog.Core.Aggregates.Categories;
using MyBlog.Core.Aggregates.Users;
using MyBlog.Core.Models;
using MyBlog.Core.Models.Blogs;
using MyBlog.Core.Repositories;
using MyBlog.Core.Services.Blogs;

namespace MyBlog.Application.Queries.Blogs.GetBlogById;

public class GetBlogByIdQueryHandler(IBlogService _blogService)
    : IRequestHandler<GetBlogByIdQuery, Result<BlogResponse>>
{
    public Task<Result<BlogResponse>> Handle(
        GetBlogByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        return _blogService.GetBlogByIdAsync(BlogId.From(request.Id), cancellationToken);
    }
}
