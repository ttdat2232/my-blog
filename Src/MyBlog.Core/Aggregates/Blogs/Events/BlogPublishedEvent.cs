using MyBlog.Core.Primitives;

namespace MyBlog.Core.Aggregates.Blogs.Events;

public record BlogPublishedEvent(Guid BlogId, string Title) : DomainEvent { }
