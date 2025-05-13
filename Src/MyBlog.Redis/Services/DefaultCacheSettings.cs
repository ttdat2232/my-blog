using Microsoft.Extensions.Configuration;
using MyBlog.Redis.Settings;

namespace MyBlog.Redis.Services;

/// <summary>
/// Default implementation of cache settings
/// </summary>
public class DefaultCacheSettings : ICacheSettings
{
    public string ConnectionString { get; }
    public TimeSpan DefaultExpiration { get; }
    public string KeyPrefix { get; }

    public DefaultCacheSettings(IConfiguration configuration)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        ConnectionString =
            configuration["Redis:ConnectionString"]
            ?? throw new InvalidOperationException("Redis connection string is not configured");

        KeyPrefix = configuration["Redis:KeyPrefix"] ?? "app";

        if (int.TryParse(configuration["Redis:DefaultExpirationMinutes"], out int minutes))
        {
            DefaultExpiration = TimeSpan.FromMinutes(minutes);
        }
        else
        {
            DefaultExpiration = TimeSpan.FromMinutes(60); // Default to 60 minutes
        }
    }
}
