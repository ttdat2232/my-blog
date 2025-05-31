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
        //TODO[improvement]: use a single query to fetch blog and comments
        //TODO[improvement]: include author information
        var blog = await _context
            .Set<BlogAggregate>()
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        if (blog is null)
            return blog;

        var sql =
            @"
            WITH RECURSIVE comment_hierachy AS (
                SELECT c.*
                FROM comments AS c
                WHERE c.blog_id = {0}
                UNION ALL
                SELECT c.*
                FROM comments AS c
                INNER JOIN comment_hierachy AS ch ON c.parent_comment_id = ch.id
            )
            SELECT * FROM comment_hierachy";
        var comments = await _context
            .Set<Comment>()
            .FromSqlRaw(sql, id.Value)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        // TODO[bug]: cannot set field
        typeof(BlogAggregate)
            .GetField(
                "_comments",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
            )
            ?.SetValue(blog, comments);
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
