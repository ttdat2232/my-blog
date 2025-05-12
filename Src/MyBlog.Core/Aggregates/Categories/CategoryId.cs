using MyBlog.Core.Primitives;

namespace MyBlog.Core.Aggregates.Categories;

public sealed class CategoryId : BaseId
{
    private CategoryId(Guid value)
        : base(value) { }

    public static new CategoryId New() => new CategoryId(Guid.NewGuid());

    public static new CategoryId From(Guid id) => new CategoryId(id);
}
