using MyBlog.Core.Primitives;

namespace MyBlog.Core.Aggregates.Users;

public sealed class UserAggregate : AggregateRoot<UserId>
{
    public string UserName { get; private set; }
    public string NormalizeUserName { get; private set; }
    public string Email { get; private set; }
    public string NormalizeEmail { get; private set; }
    public string Password { get; private set; }
    public string? Avatar { get; private set; }

    /// <summary>
    /// Get users that followed by this user
    /// </summary>
    /// <returns></returns>
    public IReadOnlyList<Subscription> Follows => _follows.AsReadOnly();

    /// <summary>
    /// Get users that followed this user
    /// </summary>
    /// <returns></returns>
    public IReadOnlyList<Subscription> FollowedBy => _followedBy.AsReadOnly();
    private readonly IList<Subscription> _follows;
    private readonly IList<Subscription> _followedBy;

    public static UserAggregate Create(
        string userName,
        string email,
        string password,
        string? avatar = null
    )
    {
        Serilog.Log.Debug(
            "Creating user aggregate with userName: {UserName}, email: {Email}, avatar: {Avatar}",
            userName,
            email,
            avatar
        );
        return new UserAggregate(UserId.New(), userName, email, password, avatar);
    }

    private UserAggregate(
        UserId id,
        string userName,
        string email,
        string password,
        string? avatar = null
    )
        : base(id)
    {
        UserName = userName;
        NormalizeUserName = userName.ToLowerInvariant();
        Email = email;
        NormalizeEmail = email.ToLowerInvariant();
        Password = password;
        Avatar = avatar;
        _followedBy = new List<Subscription>();
        _follows = new List<Subscription>();
    }

    // EF Core require
#pragma warning disable CS8618
    private UserAggregate()
#pragma warning restore CS8618
        : base(UserId.New()) { }
}
