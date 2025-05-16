using MyBlog.Core.Primitives;

namespace MyBlog.Core.Aggregates.Clients;

public class ClientId : BaseId
{
    public static new ClientId New() => new(Guid.NewGuid());

    public static new ClientId From(Guid id) => new(id);

    private ClientId(Guid id)
        : base(id) { }

    private ClientId()
        : base(Guid.Empty) { }
}
