namespace MyBlog.Core.Primitives;

public interface IEntity
{
    bool IsDeleted { get; }
    IEnumerable<DomainEvent> DomainEvents { get; }

    void AddDomainEvent(DomainEvent domainEvent);
    void ClearDomainEvent();
    void Delete();
    void RemoveDomainEvent(DomainEvent domainEvent);
}
