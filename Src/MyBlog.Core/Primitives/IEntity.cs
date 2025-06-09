namespace MyBlog.Core.Primitives;

public interface IEntity
{
    bool IsDeleted { get; }
    IEnumerable<DomainEvent> DomainEvents { get; }
    void Audit(bool isCreated, DateTime time);
    void AddDomainEvent(DomainEvent domainEvent);
    void ClearDomainEvent();
    void Delete();
    void RemoveDomainEvent(DomainEvent domainEvent);
}
