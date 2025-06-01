using MyBlog.Core.Aggregates.Blogs;
using MyBlog.Core.Models;
using MyBlog.Core.Models.Blogs;
using MyBlog.Core.Primitives;

namespace MyBlog.Core.Services.Blogs;

public interface IBlogService
{
    Task<Result<BlogAggregate>> CreateBlogAsync(
        string title,
        string content,
        Guid authorId,
        Guid categoryId,
        bool isDraft,
        DateTime? publishDate,
        CancellationToken cancellationToken
    );
    Task<Result<bool>> ValidateUpdateOperationAsync(
        BlogAggregate blog,
        BaseId requestUserId,
        BaseId? newCategoryId,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Update view count for blog
    /// </summary>
    /// <param name="viewCounts">View count dictionary include blog id and number of count will be added</param>
    /// <param name="cancellattionToken">Cancellation token</param>
    /// <returns></returns>
    Task UpdateViewCount(IDictionary<Guid, long> viewCounts, CancellationToken cancellationToken);

    Task<Result<BlogResponse>> GetBlogByIdAsync(BlogId blogId, CancellationToken cancellationToken);
}
