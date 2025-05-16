using MyBlog.Core.Aggregates.Roles;
using MyBlog.Core.Primitives;

namespace MyBlog.Core.Aggregates.Users;

public class UserRole : Entity<(UserId, RoleId)>
{
    public UserId UserId { get; private set; }
    public RoleId RoleId { get; private set; }

    public static UserRole From((UserId userid, RoleId roleId) id) => new(id);

    private UserRole((UserId userId, RoleId roleId) id)
        : base(id)
    {
        UserId = id.userId;
        RoleId = id.roleId;
    }
    //EF core require
#pragma warning disable CS8618
    private UserRole()
#pragma warning restore CS8618
        : base(default) { }
}
