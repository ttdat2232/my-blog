using MyBlog.Core.Primitives;

namespace MyBlog.Core.Aggregates.Tags;

public sealed class TagId : BaseId
{
    private TagId(Guid value)
        : base(value) { }

    public static new TagId New() => new TagId(Guid.NewGuid());

    public static new TagId From(Guid id) => new TagId(id);
}
