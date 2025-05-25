using MyBlog.Core.Primitives;

namespace MyBlog.Core.Aggregates.Categories;

public sealed class CategoryAggregate : AggregateRoot<CategoryId>
{
    private CategoryAggregate(CategoryId id, string name, string description)
        : base(id)
    {
        Name = name;
        NormalizeName = name.ToLower();
        Description = description;
    }

    public static CategoryAggregate Create(string name, string description) =>
        new(CategoryId.New(), name, description);

    public string Name { get; private set; }
    public string Description { get; private set; }

    public string NormalizeName { get; private set; }
}
