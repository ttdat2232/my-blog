using MyBlog.Core.Aggregates.Blogs;
using MyBlog.Core.Aggregates.Categories;
using MyBlog.Core.Aggregates.Users;
using MyBlog.Core.Models;
using MyBlog.Core.Models.Blogs;
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

    public async Task<Result<BlogResponse>> GetBlogByIdAsync(
        BlogId blogId,
        CancellationToken cancellationToken
    )
    {
        var blog = await _unitOfWork.BlogRepository.FindById(blogId, cancellationToken);
        if (blog is null)
            return Result<BlogResponse>.Failure(BlogErrors.NotFoundBlog);
        var userIds = CollectUserIds(blog);
        var users = await _unitOfWork
            .Repository<UserAggregate, UserId>()
            .GetAllAsync(
                u => new { u.Id, u.UserName },
                u => userIds.Contains(u.Id),
                cancellationToken
            );
        var userDict = users.ToDictionary(u => u.Id, u => u.UserName);
        var category = await _unitOfWork
            .Repository<CategoryAggregate, CategoryId>()
            .GetOneAsync(
                c => c.Id == blog.CategoryId,
                c => new { c.Name, c.Id },
                cancellationToken
            );
        return Result<BlogResponse>.Success(
            BlogResponse.FromAggregate(
                blog,
                userDict.TryGetValue(blog.AuthorId, out var authorName) ? authorName : string.Empty,
                category?.Name ?? string.Empty,
                userDict
            )
        );
    }

    private static HashSet<UserId> CollectUserIds(BlogAggregate blog)
    {
        var userIds = new HashSet<UserId> { blog.AuthorId };

        foreach (var comment in blog.Comments)
        {
            userIds.Add(comment.AuthorId);

            if (comment.ChildrenComment != null)
            {
                foreach (var childComment in comment.ChildrenComment)
                {
                    userIds.Add(childComment.AuthorId);
                }
            }
        }

        return userIds;
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
