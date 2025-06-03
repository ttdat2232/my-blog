using MyBlog.Core.Aggregates.Blogs;

namespace MyBlog.Core.Repositories;

public interface IBlogRepository : IRepository<BlogAggregate, BlogId>
{
    Task<int> UpdateViewCount(
        IDictionary<BlogId, long> viewCounts,
        CancellationToken cancellationToken = default
    );

    Task<BlogAggregate?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
}
