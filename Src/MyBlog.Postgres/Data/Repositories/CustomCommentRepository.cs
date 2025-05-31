using Microsoft.EntityFrameworkCore;
using MyBlog.Core.Aggregates.Blogs;
using MyBlog.Core.Primitives;
using MyBlog.Core.Repositories;

namespace MyBlog.Postgres.Data.Repositories;

public class CustomCommentRepository : Repository<Comment, BaseId>, ICommentRepository
{
    private readonly MyBlogContext _dbContext;

    public CustomCommentRepository(MyBlogContext dbContext)
        : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Comment>> GetCommentsByBlogIdAsync(
        BlogId blogId,
        CancellationToken cancellationToken = default
    )
    {
        var sql =
            @"
            WITH RECURSIVE comment_hierachy AS (
                SELECT c.*
                FROM comments AS c
                WHERE r.id = {0}
                UNION ALL
                SELECT r.*
                FROM reply AS r
                INNER JOIN comment_hierachy AS ch ON r.parent_comment_id = ch.id
            )
            SELECT * FROM comment_hierachy";
        return await _dbContext
            .Set<Comment>()
            .FromSqlRaw(sql, blogId.Value)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
