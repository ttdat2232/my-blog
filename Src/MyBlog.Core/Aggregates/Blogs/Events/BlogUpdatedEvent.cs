using MyBlog.Core.Primitives;

namespace MyBlog.Core.Aggregates.Blogs.Events;

public class BlogUpdatedEvent(BlogAggregate BlogAggregate) : DomainEvent;
