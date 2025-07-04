using MyBlog.Core.Aggregates.Blogs.Events;
using MyBlog.Core.Aggregates.Categories;
using MyBlog.Core.Aggregates.Users;
using MyBlog.Core.Models;
using MyBlog.Core.Primitives;
using Serilog;

namespace MyBlog.Core.Aggregates.Blogs;

public sealed class BlogAggregate : AggregateRoot<BlogId>
{
    public string Title { get; private set; }
    public string Content { get; private set; }
    public long ViewCount { get; private set; }
    public string Slug { get; private set; }
    public UserId AuthorId { get; private set; }
    public CategoryId CategoryId { get; private set; }
    public BlogStatus Status { get; private set; }
    public DateTime? PublishDate { get; private set; }
    public bool IsPublished { get; private set; }
    public IEnumerable<Comment> Comments => _comments.AsReadOnly();
    public IEnumerable<BlogTag> Tags => _tags.AsReadOnly();
    public IEnumerable<Like> Likes => _likes.AsReadOnly();
    private readonly IList<Like> _likes;
    private readonly IList<BlogTag> _tags;
    private readonly IList<Comment> _comments;

    public static Result<BlogAggregate> Create(
        string title,
        string content,
        Guid authorId,
        Guid categoryId,
        bool isDraft,
        int slugCount,
        DateTime? publishDate
    )
    {
        Log.Debug(
            "Creating blog aggregate with title: {Title}, content: {Content}, authorId: {AuthorId}",
            title,
            content,
            authorId
        );

        if (publishDate.HasValue && publishDate.Value < DateTime.UtcNow)
        {
            return Result<BlogAggregate>.Failure(BlogErrors.InvalidPublishDate);
        }
        var status = isDraft ? BlogStatus.Draft : BlogStatus.Active;
        var slug = GetSlug(title);
        if (slugCount > 0)
        {
            slug = $"{slug}-{slugCount}";
        }
        var blog = new BlogAggregate(
            BlogId.New(),
            title,
            content,
            UserId.From(authorId),
            CategoryId.From(categoryId),
            slug,
            status,
            publishDate,
            publishDate.HasValue
        );
        return Result<BlogAggregate>.Success(blog);
    }

    public static string GetSlug(string title) =>
        title
            .ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("'", "")
            .Replace("\"", "")
            .Replace(",", "")
            .Replace(".", "")
            .Replace("!", "")
            .Replace("?", "")
            .Replace(":", "")
            .Replace(";", "");

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

    public Result<bool> Update(string title, string content, BlogStatus status, int slugCount)
    {
        if (Status == BlogStatus.Banned)
            return Result<bool>.Failure(BlogErrors.ActionOnBannedBlog);

        Title = title;
        Slug = GetSlug(title);
        if (slugCount > 0)
        {
            Slug = $"{Slug}-{slugCount}";
        }
        Content = content;
        Status = status;
        AddDomainEvent(new BlogUpdatedEvent(this));
        return Result<bool>.Success(true);
    }

    public void ChangeCategory(Guid newCategoryId)
    {
        if (newCategoryId != CategoryId)
        {
            CategoryId = CategoryId.From(newCategoryId);
        }
    }

    public void AddComment(string content, Guid authorId, Guid? parentId = null)
    {
        var comment = Comment.Create(Id, content, authorId, parentId);
        AddDomainEvent(new BlogCommentAddedEvent(Id, comment.AuthorId, comment.Content));
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

    public void IncreaseView(long count)
    {
        ViewCount += count;
    }

    public void AddLike(Guid userId)
    {
        var userIdFrom = UserId.From(userId);
        if (Likes.Any(l => l.UserId == userIdFrom))
        {
            Log.Warning("User {UserId} already liked the blog {BlogId}", userId, Id.Value);
            return;
        }
        _likes.Add(Like.Create(Id, userIdFrom));
        Log.Debug("User {UserId} liked the blog {BlogId}", userId, Id.Value);
    }

    private BlogAggregate(
        BlogId id,
        string title,
        string content,
        UserId authorId,
        CategoryId categoryId,
        string slug,
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
        PublishDate = publishDate;
        Status = status;
        IsPublished = isPublish;
        Slug = slug;
        _tags = new List<BlogTag>();
        _comments = new List<Comment>();
        _likes = new List<Like>();
        AddDomainEvent(new BlogCreatedEvent(id, Title, AuthorId));
    }

    // EF core require
#pragma warning disable CS8618, CS8625
    private BlogAggregate()
        : base(default)
    {
        _tags = new List<BlogTag>();
        _comments = new List<Comment>();
        _likes = new List<Like>();
    }
#pragma warning restore CS8618, CS8625
}
