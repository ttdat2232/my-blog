using MyBlog.Core.Primitives;

namespace MyBlog.Core.Aggregates.Blogs.Events;

public class BlogPublishedEvent(Guid BlogId, string Title) : DomainEvent { }
