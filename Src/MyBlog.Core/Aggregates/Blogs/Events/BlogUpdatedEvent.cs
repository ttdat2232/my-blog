using MyBlog.Core.Primitives;

namespace MyBlog.Core.Aggregates.Blogs.Events;

public record BlogUpdatedEvent(BlogAggregate BlogAggregate) : DomainEvent;
