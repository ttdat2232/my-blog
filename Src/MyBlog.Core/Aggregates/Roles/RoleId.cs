using MyBlog.Core.Primitives;

namespace MyBlog.Core.Aggregates.Roles;

public class RoleId : BaseId
{
    private RoleId(Guid value)
        : base(value) { }

    public static new RoleId New() => new RoleId(Guid.NewGuid());

    public static new RoleId From(Guid id) => new RoleId(id);
}
