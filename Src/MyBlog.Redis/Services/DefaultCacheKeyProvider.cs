using MyBlog.Core.Services.Cache;
using MyBlog.Redis.Settings;

namespace MyBlog.Redis.Services;

/// <summary>
/// Default implementation of cache key provider
/// </summary>
public class DefaultCacheKeyProvider : ICacheKeyProvider
{
    private readonly ICacheSettings _settings;

    public DefaultCacheKeyProvider(ICacheSettings settings)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public string GenerateKey(params string[] keyParts)
    {
        if (keyParts == null || keyParts.Length == 0)
        {
            throw new ArgumentException("At least one key part must be provided", nameof(keyParts));
        }

        return $"{_settings.KeyPrefix}:{string.Join(":", keyParts)}";
    }
}
