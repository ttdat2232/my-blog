using MyBlog.Core.Primitives;

namespace MyBlog.Core.Aggregates.Blogs.Events;

public class BlogCommentAddedEvent(Guid BlogId, Comment AddedComment) : DomainEvent { }
