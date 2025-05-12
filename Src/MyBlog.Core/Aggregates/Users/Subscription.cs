using MyBlog.Core.Primitives;

namespace MyBlog.Core.Aggregates.Users;

public sealed class Subscription : Entity<(UserId, UserId)>
{
    public UserId FollowerId { get; private set; }
    public UserId FollowedId { get; private set; }

    private Subscription(Guid followerId, Guid followedId)
        : base((UserId.From(followerId), UserId.From(followedId)))
    {
        FollowerId = UserId.From(followerId);
        FollowedId = UserId.From(followedId);
    }

    //EF core require
#pragma warning disable CS8618
    private Subscription()
#pragma warning restore CS8618
#pragma warning disable CS8620
        : base((default, default)) { }
#pragma warning restore CS8620
}
