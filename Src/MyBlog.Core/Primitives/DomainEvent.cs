using MediatR;

namespace MyBlog.Core.Primitives;

public abstract class DomainEvent : INotification
{
    protected DomainEvent()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public DateTime CreatedAt { get; }
}
