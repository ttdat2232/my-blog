using MyBlog.Core.Aggregates.Users;
using MyBlog.Core.Primitives;

namespace MyBlog.Core.Aggregates.Blogs;

public class Like : Entity<(BlogId, UserId)>
{
    public BlogId BlogId { get; set; }
    public UserId UserId { get; set; }

    public static Like Create(BlogId blogId, UserId userId) => new Like(blogId, userId);

    private Like(BlogId blogId, UserId userId)
        : base((blogId, userId))
    {
        BlogId = blogId;
        UserId = userId;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Like()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        : base((BlogId.From(Guid.Empty), UserId.From(Guid.Empty))) { }
}
