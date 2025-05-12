using MyBlog.Core.Primitives;

namespace MyBlog.Core.Aggregates.Blogs.Events;

public class BlogCreatedEvent(Guid Id, string Title, Guid AuthorId) : DomainEvent { }
