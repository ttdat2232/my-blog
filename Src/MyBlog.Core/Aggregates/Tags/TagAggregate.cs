using MyBlog.Core.Primitives;

namespace MyBlog.Core.Aggregates.Tags;

public sealed class TagAggregate : AggregateRoot<TagId>
{
    private TagAggregate(TagId id, string name)
        : base(id)
    {
        Name = name;
    }

    public static TagAggregate Create(string name) => new(TagId.New(), name);

    public string Name { get; private set; }
}
