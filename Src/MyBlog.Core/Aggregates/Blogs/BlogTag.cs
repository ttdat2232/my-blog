using MyBlog.Core.Aggregates.Tags;
using MyBlog.Core.Primitives;

namespace MyBlog.Core.Aggregates.Blogs;

public sealed class BlogTag : Entity<(BaseId, BaseId)>
{
    public BlogId BlogId { get; private set; }
    public TagId TagId { get; private set; }

    public static BlogTag From(BlogId blogId, Guid tagId) => new BlogTag(blogId, TagId.From(tagId));

    public static BlogTag From(Guid blogId, Guid tagId) =>
        new BlogTag(BlogId.From(blogId), TagId.From(tagId));

    private BlogTag(BlogId blogId, TagId tagId)
        : base((blogId, tagId))
    {
        BlogId = blogId;
        TagId = tagId;
    }
    // EF core require
#pragma warning disable CS8618
    private BlogTag()
#pragma warning disable CS8620
        : base((default, default)) { }
#pragma warning restore CS8620
}
