using MyBlog.Core.Primitives;

namespace MyBlog.Core.Aggregates.Blogs;

public sealed class BlogId : BaseId
{
    private BlogId(Guid value)
        : base(value) { }

    public static new BlogId New() => new BlogId(Guid.NewGuid());

    public static new BlogId From(Guid id) => new BlogId(id);
}
