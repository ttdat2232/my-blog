using MyBlog.Core.Aggregates.Blogs.Events;
using MyBlog.Core.Models;
using MyBlog.Core.Primitives;
using Serilog;

namespace MyBlog.Core.Aggregates.Blogs;

public sealed class BlogAggregate : AggregateRoot<BlogId>
{
    private BlogAggregate(
        BlogId id,
        string title,
        string content,
        BaseId authorId,
        BaseId categoryId,
        BlogStatus status,
        DateTime? publishDate,
        bool isPublish
    )
        : base(id)
    {
        Title = title;
        Content = content;
        ViewCount = 0;
        AuthorId = authorId;
        CategoryId = categoryId;
        _tags = new List<BlogTag>();
        _comments = new List<Comment>();
        PublishDate = publishDate;
        Status = status;
        IsPublished = isPublish;
        AddDomainEvent(new BlogCreatedEvent(id, Title, AuthorId));
    }

    // EF core require
#pragma warning disable CS8618, CS8625
    private BlogAggregate()
        : base(default) { }
#pragma warning restore CS8618, CS8625

    public string Title { get; private set; }
    public string Content { get; private set; }
    public long ViewCount { get; private set; }
    public BaseId AuthorId { get; private set; }
    public BaseId CategoryId { get; private set; }
    public BlogStatus Status { get; private set; }
    public DateTime? PublishDate { get; private set; }
    public bool IsPublished { get; private set; }
    public IEnumerable<Comment> Comments => _comments.AsReadOnly();
    public IEnumerable<BlogTag> Tags => _tags.AsReadOnly();
    private readonly IList<BlogTag> _tags;
    private readonly IList<Comment> _comments;

    public static Result<BlogAggregate> Create(
        string title,
        string content,
        BaseId authorId,
        BaseId categoryId,
        bool isDraft,
        DateTime? publishDate
    )
    {
        Log.Debug(
            "Creating blog aggregate with title: {Title}, content: {Content}, authorId: {AuthorId}",
            title,
            content,
            authorId
        );

        var status = isDraft ? BlogStatus.Draft : BlogStatus.Active;

        var blog = new BlogAggregate(
            BlogId.New(),
            title,
            content,
            authorId,
            categoryId,
            status,
            publishDate,
            publishDate.HasValue
        );
        return Result<BlogAggregate>.Success(blog);
    }

    public Result<bool> Publish()
    {
        if (IsPublished)
            return Result<bool>.Failure(BlogErrors.AlreadyPublish);
        if (Status == BlogStatus.Draft)
            return Result<bool>.Failure(BlogErrors.PublishDraft);

        PublishDate = DateTime.UtcNow;
        IsPublished = true;
        Status = BlogStatus.Active;
        return Result<bool>.Success(true);
    }

    public void AddTag(Guid tagId)
    {
        _tags.Add(BlogTag.From(Id, tagId));
    }

    public void RemoveTag(Guid tagId)
    {
        _tags.Remove(BlogTag.From(Id, tagId));
    }

    public Result<bool> Update(string title, string content, BlogStatus status)
    {
        if (Status == BlogStatus.Banned)
            return Result<bool>.Failure(BlogErrors.ActionOnBannedBlog);

        Title = title;
        Content = content;
        Status = status;
        AddDomainEvent(new BlogUpdatedEvent(this));
        return Result<bool>.Success(true);
    }

    public void ChangeCategory(BaseId newCategoryId)
    {
        if (newCategoryId != CategoryId)
        {
            CategoryId = newCategoryId;
        }
    }

    public void AddComment(string content, BaseId authorId, BaseId? parentId = null)
    {
        var comment = Comment.Create(Id, content, authorId, parentId);
        _comments.Add(comment);
        Log.Debug("New comment was added");
    }

    public void RemoveComment(Comment removingComment)
    {
        if (_comments.Remove(removingComment))
        {
            removingComment.Delete();
            Log.Debug("a comment was removed");
        }
    }
}
