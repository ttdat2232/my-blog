using MediatR;

namespace MyBlog.Core.Primitives;

public record DomainEvent : INotification
{
    protected DomainEvent()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public DateTime CreatedAt { get; }
}
