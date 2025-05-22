using Microsoft.EntityFrameworkCore;
using MyBlog.Core.Aggregates.Blogs;
using MyBlog.Core.Repositories;

namespace MyBlog.Postgres.Data.Repositories;

public class CustomBlogRepository(MyBlogContext _context)
    : Repository<BlogAggregate, BlogId>(_context),
        IBlogRepository
{
    public Task<int> UpdateViewCount(
        IDictionary<BlogId, long> viewCounts,
        CancellationToken cancellationToken = default
    )
    {
        _context
            .Set<BlogAggregate>()
            .Where(b => viewCounts.Any(view => view.Key == b.Id))
            .ExecuteUpdate(b =>
                b.SetProperty(b => b.ViewCount, b => b.ViewCount + viewCounts[b.Id])
            );
        return _context.SaveChangesAsync(cancellationToken);
    }
}
