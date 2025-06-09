using System.Security.Cryptography;
using System.Text;
using MyBlog.Core.Aggregates.Roles;
using MyBlog.Core.Aggregates.Users.Events;
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

    public IReadOnlyList<UserRole> Roles => _roles.AsReadOnly();

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

    private readonly IList<UserRole> _roles;
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
        var hashedPassword = HashPassword(password);
        return new UserAggregate(UserId.New(), userName, email, hashedPassword, avatar);
    }

    private static string HashPassword(string password)
    {
        // Generate a random salt
        byte[] salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // Create hash
        using var sha256 = SHA256.Create();
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var saltedPassword = new byte[passwordBytes.Length + salt.Length];
        Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
        Buffer.BlockCopy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

        var hashedBytes = sha256.ComputeHash(saltedPassword);

        // Combine salt and hash
        var hashWithSalt = new byte[hashedBytes.Length + salt.Length];
        Buffer.BlockCopy(hashedBytes, 0, hashWithSalt, 0, hashedBytes.Length);
        Buffer.BlockCopy(salt, 0, hashWithSalt, hashedBytes.Length, salt.Length);

        return Convert.ToBase64String(hashWithSalt);
    }

    public bool ValidatePassword(string password)
    {
        try
        {
            // Extract salt from stored hash
            byte[] hashWithSalt = Convert.FromBase64String(Password);
            byte[] salt = new byte[16];
            Buffer.BlockCopy(hashWithSalt, 32, salt, 0, 16);

            // Hash the input password with the same salt
            using var sha256 = SHA256.Create();
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var saltedPassword = new byte[passwordBytes.Length + salt.Length];
            Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
            Buffer.BlockCopy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

            var hashedBytes = sha256.ComputeHash(saltedPassword);

            // Compare the hashes
            byte[] storedHash = new byte[32];
            Buffer.BlockCopy(hashWithSalt, 0, storedHash, 0, 32);

            return storedHash.SequenceEqual(hashedBytes);
        }
        catch
        {
            return false;
        }
    }

    public void AssignRole(params RoleId[] roleIds)
    {
        foreach (var roleId in roleIds)
            _roles.Add(UserRole.From((Id, roleId)));
    }

    public void RemoveRoles(params RoleId[] roleIds)
    {
        foreach (var roleId in roleIds)
            _roles.Remove(UserRole.From((Id, roleId)));
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
        _roles = new List<UserRole>();
        AddDomainEvent(new UserCreatedEvent(this));
    }

    public void AddRole(RoleAggregate role)
    {
        if (role is null)
            throw new ArgumentNullException(nameof(role));

        if (_roles.Any(r => r.RoleId.Equals(role.Id)))
            return;

        _roles.Add(UserRole.From((Id, role.Id)));
    }

    // EF Core require
#pragma warning disable CS8618
    private UserAggregate()
#pragma warning restore CS8618
        : base(UserId.New())
    {
        _followedBy = new List<Subscription>();
        _follows = new List<Subscription>();
        _roles = new List<UserRole>();
    }
}
