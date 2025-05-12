using MyBlog.Core.Aggregates.Users;
using MyBlog.Core.Primitives;

namespace MyBlog.Core.Aggregates.Blogs;

public class Comment : Entity<BaseId>
{
    public BaseId? ParentCommentId { get; private set; }
    public BlogId BlogId { get; private set; }
    public string Content { get; private set; }
    public UserId AuthorId { get; private set; }
    public IEnumerable<Comment> ChidrenComment => _childrenComment.AsReadOnly();
    private readonly IList<Comment> _childrenComment;

    public static Comment Create(
        BaseId blogId,
        string content,
        BaseId authorId,
        BaseId? parentCommentId = null
    )
    {
        return new Comment(BaseId.New(), blogId, content, authorId, parentCommentId);
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
        : base(default) { }
#pragma warning restore CS8618, CS8625

    private Comment(BaseId id, Guid blogId, string content, Guid authorId, BaseId? parentCommentId)
        : base(id)
    {
        BlogId = BlogId.From(blogId);
        Content = content;
        AuthorId = UserId.From(authorId);
        ParentCommentId = parentCommentId;
        _childrenComment = new List<Comment>();
    }
}
