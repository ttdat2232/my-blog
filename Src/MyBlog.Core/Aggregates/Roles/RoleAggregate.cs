using MyBlog.Core.Primitives;

namespace MyBlog.Core.Aggregates.Roles;

public class RoleAggregate : AggregateRoot<RoleId>
{
    public string Name { get; private set; }
    public string NormalizeName { get; private set; }
    public string? Description { get; private set; }

    public static RoleAggregate Create(string name, string? description = null) =>
        new(name, description);

    private RoleAggregate(string name, string? description)
        : base(RoleId.New())
    {
        Name = name;
        NormalizeName = name.ToUpper();
        Description = description;
    }
    // EF core require
#pragma warning disable CS8618
    private RoleAggregate()
#pragma warning restore CS8618
        : base(RoleId.New()) { }
}
