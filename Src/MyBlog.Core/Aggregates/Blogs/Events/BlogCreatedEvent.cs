using MyBlog.Core.Primitives;

namespace MyBlog.Core.Aggregates.Blogs.Events;

public record BlogCreatedEvent(Guid Id, string Title, Guid AuthorId) : DomainEvent { }
