using MyBlog.Core.Primitives;

namespace MyBlog.Core.Aggregates.Blogs.Events;

public record BlogCommentAddedEvent(Guid BlogId, Guid AuthorId, string Content) : DomainEvent { }
