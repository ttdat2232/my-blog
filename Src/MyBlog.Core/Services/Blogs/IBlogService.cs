using MyBlog.Core.Aggregates.Blogs;
using MyBlog.Core.Models;
using MyBlog.Core.Primitives;

namespace MyBlog.Core.Services.Blogs;

public interface IBlogService
{
    Task<Result<BlogAggregate>> CreateBlogAsync(
        string title,
        string content,
        BaseId authorId,
        BaseId categoryId,
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
}
