using MyBlog.Core.Aggregates.Blogs;
using MyBlog.Core.Primitives;

namespace MyBlog.Core.Repositories;

public interface ICommentRepository : IRepository<Comment, BaseId>
{
    Task<IEnumerable<Comment>> GetCommentsByBlogIdAsync(
        BlogId blogId,
        CancellationToken cancellationToken = default
    );
}
