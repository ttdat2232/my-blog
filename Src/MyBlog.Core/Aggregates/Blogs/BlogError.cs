using MyBlog.Core.Models;

namespace MyBlog.Core.Aggregates.Blogs;

public static class BlogErrors
{
    public static readonly Error PublishDraft = new("Not allow to publish draft blog", 400);

    public static readonly Error AlreadyPublish = new("Blog was already published", 400);

    public static readonly Error AuthorNotExisted = new("Counldn't found author", 400);
    public static readonly Error CategoryNotExisted = new("Counldn't found category", 400);
    public static readonly Error NotBlogOwner = new(
        "Action not allow because blog is not belong to current user",
        400
    );
    public static readonly Error ActionOnBannedBlog = new("Action not allow with banned blog", 400);
    public static readonly Error InvalidPublishDate = new(
        "Publish date cannot be before the current time",
        400
    );
}
