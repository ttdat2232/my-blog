namespace MyBlog.Core.Primitives;

public abstract class Entity<T> : AutditableEntity, IEquatable<Entity<T>>
{
    protected Entity(T id)
    {
        _id = id;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        _domainEvents = new List<DomainEvent>();
    }

    public virtual T Id => _id;
    public bool IsDeleted { get; private set; }

    public IEnumerable<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    private readonly IList<DomainEvent> _domainEvents;
    private readonly T _id;

    public void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvent()
    {
        _domainEvents.Clear();
    }

    public static bool operator ==(Entity<T>? left, Entity<T>? right)
    {
        if (left is null && right is null)
            return true;
        if (left is null || right is null)
            return false;
        return left.Equals(right);
    }

    public static bool operator !=(Entity<T>? left, Entity<T>? right)
    {
        return !(left == right);
    }

    public override int GetHashCode()
    {
        return _id!.GetHashCode() * 907;
    }

    public bool Equals(Entity<T>? other)
    {
        return Equals(other as object);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (GetType() != obj.GetType())
            return false;
        if (obj is not Entity<T> entity)
            return false;
        return _id!.Equals(entity._id);
    }

    public virtual void Delete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }
}
