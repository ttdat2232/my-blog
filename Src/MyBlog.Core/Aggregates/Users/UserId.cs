using MyBlog.Core.Primitives;

namespace MyBlog.Core.Aggregates.Users;

public sealed class UserId : BaseId
{
    private UserId(Guid value)
        : base(value) { }

    public static new UserId New() => new UserId(Guid.NewGuid());

    public static new UserId From(Guid id) => new UserId(id);

    public static new UserId From(string id) => new UserId(Guid.Parse(id));
}
