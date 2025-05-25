using MyBlog.Core.Aggregates.Blogs;
using MyBlog.Core.Aggregates.Categories;
using MyBlog.Core.Aggregates.Users;
using MyBlog.Core.Models;
using MyBlog.Core.Primitives;
using MyBlog.Core.Repositories;

namespace MyBlog.Core.Services.Blogs;

public class BlogService(IUnitOfWork _unitOfWork) : IBlogService
{
    public async Task<Result<BlogAggregate>> CreateBlogAsync(
        string title,
        string content,
        Guid authorId,
        Guid categoryId,
        bool isDraft,
        DateTime? publishDate,
        CancellationToken cancellationToken
    )
    {
        Serilog.Log.Debug(
            "Create blog with title: {Title}, authorId: {AuthorId}, categoryId: {CategoryId}, isDraft: {IsDraft}, publishDate: {PublishDate}",
            title,
            authorId,
            categoryId,
            isDraft,
            publishDate
        );
        var _userRepository = _unitOfWork.Repository<UserAggregate, UserId>();
        var _categoryRepository = _unitOfWork.Repository<CategoryAggregate, CategoryId>();

        if (!await _userRepository.IsExistedAsync(UserId.From(authorId), cancellationToken))
            return Result<BlogAggregate>.Failure(BlogErrors.AuthorNotExisted);

        if (
            !await _categoryRepository.IsExistedAsync(
                CategoryId.From(categoryId),
                cancellationToken
            )
        )
            return Result<BlogAggregate>.Failure(BlogErrors.CategoryNotExisted);

        var blogResult = BlogAggregate.Create(
            title,
            content,
            authorId,
            categoryId,
            isDraft,
            publishDate
        );

        return blogResult;
    }

    public async Task UpdateViewCount(
        IDictionary<Guid, long> viewCounts,
        CancellationToken cancellationToken
    )
    {
        Serilog.Log.Debug("Update view for blog with {Count} items", viewCounts?.Count ?? 0);
        if (viewCounts == null)
            return;
        await _unitOfWork.BlogRepository.UpdateViewCount(
            viewCounts.ToDictionary(view => BlogId.From(view.Key), view => view.Value)
        );
    }

    public async Task<Result<bool>> ValidateUpdateOperationAsync(
        BlogAggregate blog,
        BaseId requestUserId,
        BaseId? newCategoryId,
        CancellationToken cancellationToken
    )
    {
        Serilog.Log.Debug(
            "Validate blog for blogId: {BlogId}, requestUserId: {RequestUserId}, newCategoryId: {NewCategoryId}",
            blog?.Id,
            requestUserId,
            newCategoryId
        );
        if (blog == null)
            return Result<bool>.Failure(BlogErrors.NotBlogOwner);
        if (blog.AuthorId != requestUserId)
            return Result<bool>.Failure(BlogErrors.NotBlogOwner);

        if (
            newCategoryId?.Value != null
            && !await _unitOfWork
                .Repository<CategoryAggregate, CategoryId>()
                .IsExistedAsync(CategoryId.From(newCategoryId.Value), cancellationToken)
        )
            return Result<bool>.Failure(BlogErrors.CategoryNotExisted);
        return Result<bool>.Success(true);
    }
}
