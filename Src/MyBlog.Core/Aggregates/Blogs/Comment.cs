using MyBlog.Core.Aggregates.Users;
using MyBlog.Core.Primitives;

namespace MyBlog.Core.Aggregates.Blogs;

public class Comment : Entity<BaseId>
{
    public BaseId? ParentCommentId { get; private set; }
    public BlogId BlogId { get; private set; }
    public string Content { get; private set; }
    public UserId AuthorId { get; private set; }
    public IEnumerable<Comment> ChildrenComment => _childrenComment.AsReadOnly();
    private readonly IList<Comment> _childrenComment;

    public static Comment Create(
        Guid blogId,
        string content,
        Guid authorId,
        Guid? parentCommentId = null
    )
    {
        return new Comment(
            BaseId.New(),
            BlogId.From(blogId),
            content,
            UserId.From(authorId),
            parentCommentId is not null ? BaseId.From(parentCommentId.Value) : null
        );
    }

    public void Update(string content)
    {
        Content = content;
    }

    public override void Delete()
    {
        Content = "Deleted";
        base.Delete();
    }
    // EF core require
#pragma warning disable CS8618, CS8625
    private Comment()
        : base(default)
    {
        _childrenComment = new List<Comment>();
    }
#pragma warning restore CS8618, CS8625

    private Comment(
        BaseId id,
        BlogId blogId,
        string content,
        UserId authorId,
        BaseId? parentCommentId
    )
        : base(id)
    {
        BlogId = blogId;
        Content = content;
        AuthorId = authorId;
        ParentCommentId = parentCommentId;
        _childrenComment = new List<Comment>();
    }
}
