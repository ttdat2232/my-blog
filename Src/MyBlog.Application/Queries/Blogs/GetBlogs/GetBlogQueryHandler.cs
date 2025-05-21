using MediatR;
using MyBlog.Application.Models;
using MyBlog.Application.Specifications.Blogs;
using MyBlog.Core.Aggregates.Blogs;
using MyBlog.Core.Aggregates.Categories;
using MyBlog.Core.Aggregates.Users;
using MyBlog.Core.Models;
using MyBlog.Core.Repositories;

namespace MyBlog.Application.Queries.Blogs.GetBlogs;

public class GetBlogsQueryHandler(IUnitOfWork _unitOfWork)
    : IRequestHandler<GetBlogsQuery, Result<PaginationResult<BlogResponse>>>
{
    public async Task<Result<PaginationResult<BlogResponse>>> Handle(
        GetBlogsQuery request,
        CancellationToken cancellationToken
    )
    {
        var blogRepo = _unitOfWork.Repository<BlogAggregate, BlogId>();
        var searchBlogSpec = new SearchBlogSpecification(
            request.Title,
            request.Status,
            request.PageNumber,
            request.PageSize
        );
        var blogs = await blogRepo.GetAsync<BlogAggregate>(searchBlogSpec, cancellationToken);

        var authorIds = blogs.Select(b => b.AuthorId).Distinct().ToList();
        var categoryIds = blogs.Select(b => b.CategoryId).Distinct().ToList();

        var userRepo = _unitOfWork.Repository<UserAggregate, UserId>();
        var categoryRepo = _unitOfWork.Repository<CategoryAggregate, CategoryId>();

        var authors = await userRepo.GetAllAsync(u => authorIds.Contains(u.Id), cancellationToken);
        var categories = await categoryRepo.GetAllAsync(
            c => categoryIds.Contains(c.Id),
            cancellationToken
        );

        var authorDict = authors.ToDictionary(a => a.Id, a => a.UserName);
        var categoryDict = categories.ToDictionary(c => c.Id, c => c.Name);

        var blogResponses = blogs
            .Select(blog => new BlogResponse(
                blog.Id,
                blog.Title,
                blog.Content,
                authorDict.TryGetValue(UserId.From(blog.AuthorId), out var authorName)
                    ? authorName
                    : string.Empty,
                categoryDict.TryGetValue(CategoryId.From(blog.CategoryId), out var categoryName)
                    ? categoryName
                    : string.Empty,
                blog.CreatedAt,
                blog.PublishDate,
                blog.ViewCount
            ))
            .ToList();

        return Result<PaginationResult<BlogResponse>>.Success(
            PaginationResult<BlogResponse>.New(
                blogResponses,
                request.PageSize,
                request.PageNumber,
                await blogRepo.CountAsync(searchBlogSpec.Criteria, cancellationToken)
            )
        );
    }
}
