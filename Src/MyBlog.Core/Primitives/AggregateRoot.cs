namespace MyBlog.Core.Primitives;

public abstract class AggregateRoot<T> : Entity<T>
    where T : BaseId
{
    public new T Id { get; private set; }

    protected AggregateRoot(T id)
        : base(id)
    {
        Id = id;
    }
}
