namespace MyBlog.Redis.Settings;

public interface ICacheSettings
{
    /// <summary>
    /// Connection string for Redis
    /// </summary>
    string ConnectionString { get; }

    /// <summary>
    /// Default expiration time for cache entries
    /// </summary>
    TimeSpan DefaultExpiration { get; }

    /// <summary>
    /// Prefix for cache keys
    /// </summary>
    string KeyPrefix { get; }
}
