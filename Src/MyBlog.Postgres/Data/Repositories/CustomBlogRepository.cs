using Microsoft.EntityFrameworkCore;
using MyBlog.Core.Aggregates.Blogs;
using MyBlog.Core.Repositories;

namespace MyBlog.Postgres.Data.Repositories;

public class CustomBlogRepository(MyBlogContext _context)
    : Repository<BlogAggregate, BlogId>(_context),
        IBlogRepository
{
    public override async Task<BlogAggregate?> FindById(
        BlogId id,
        CancellationToken cancellationToken = default
    )
    {
        var blog = await _context
            .Set<BlogAggregate>()
            .Include(b => b.Comments.Where(c => c.ParentCommentId == null))
            .ThenInclude(c => c.ChildrenComment)
            .Include(b => b.Likes)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        return blog;
    }

    public async Task<BlogAggregate?> GetBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default
    )
    {
        var blog = await _context
            .Set<BlogAggregate>()
            .Include(b => b.Comments.Where(blog => blog.ParentCommentId == null))
            .ThenInclude(c => c.ChildrenComment)
            .Include(b => b.Likes)
            .FirstOrDefaultAsync(b => b.Slug == slug);
        return blog;
    }

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
