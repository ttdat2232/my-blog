using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MyBlog.Core.Primitives;

namespace MyBlog.Infrastructure.Data.Interceptors;

public class PublishDomainEventInterceptor(IPublisher _publisher) : SaveChangesInterceptor
{
    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default
    )
    {
        if (eventData.Context != null)
        {
            await PublishDomainEvent(eventData.Context);
        }
        return result;
    }

    private async Task PublishDomainEvent(DbContext context)
    {
        var domainEvents = context
            .ChangeTracker.Entries<Entity<object>>()
            .Select(e => e.Entity)
            .SelectMany(entity =>
            {
                List<DomainEvent> domainEvents = (List<DomainEvent>)entity.DomainEvents;
                entity.ClearDomainEvent();
                return domainEvents;
            });
        foreach (var @event in domainEvents)
        {
            await _publisher.Publish(@event);
        }
    }
}
