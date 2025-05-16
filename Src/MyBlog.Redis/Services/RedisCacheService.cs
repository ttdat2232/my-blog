using System.Text.Json;
using MyBlog.Core.Services.Cache;
using MyBlog.Redis.Settings;
using Serilog;

namespace MyBlog.Redis.Services;

/// <summary>
/// Redis implementation of the cache service
/// </summary>
/// <typeparam name="T">Type of cached items</typeparam>
public class RedisCacheService : ICacheService
{
    private readonly RedisCacheConnectionProvider _connectionProvider;
    private readonly ICacheKeyProvider _keyProvider;
    private readonly ICacheSettings _settings;

    public RedisCacheService(
        RedisCacheConnectionProvider connectionProvider,
        ICacheKeyProvider keyProvider,
        ICacheSettings settings
    )
    {
        _connectionProvider =
            connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
        _keyProvider = keyProvider ?? throw new ArgumentNullException(nameof(keyProvider));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var formattedKey = _keyProvider.GenerateKey(typeof(T).Name, key);
            var database = _connectionProvider.GetDatabase();
            var value = await database.StringGetAsync(formattedKey);

            if (value.IsNull)
            {
                Log.Debug("Cache miss for key: {Key}", formattedKey);
                return default;
            }

            Log.Debug("Cache hit for key: {Key}", formattedKey);
            return JsonSerializer.Deserialize<T>(value.ToString());
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            Log.Error(ex, "Error getting value from cache for key: {Key}", key);
            return default;
        }
    }

    public async Task<bool> SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiry = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var formattedKey = _keyProvider.GenerateKey(typeof(T).Name, key);
            var database = _connectionProvider.GetDatabase();
            var serializedValue = JsonSerializer.Serialize(value);

            var actualExpiry = expiry ?? _settings.DefaultExpiration;

            var result = await database.StringSetAsync(formattedKey, serializedValue, actualExpiry);

            if (result)
            {
                Log.Debug(
                    "Successfully cached item with key: {Key} and expiry: {Expiry}",
                    formattedKey,
                    actualExpiry
                );
            }
            else
            {
                Log.Warning("Failed to cache item with key: {Key}", formattedKey);
            }

            return result;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            Log.Error(ex, "Error setting value in cache for key: {Key}", key);
            return false;
        }
    }

    public async Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var formattedKey = _keyProvider.GenerateKey(key);
            var database = _connectionProvider.GetDatabase();

            var result = await database.KeyDeleteAsync(formattedKey);

            if (result)
            {
                Log.Debug("Successfully removed item with key: {Key}", formattedKey);
            }
            else
            {
                Log.Debug("Item with key: {Key} not found for removal", formattedKey);
            }

            return result;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            Log.Error(ex, "Error removing value from cache for key: {Key}", key);
            return false;
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var formattedKey = _keyProvider.GenerateKey(key);
            var database = _connectionProvider.GetDatabase();

            return await database.KeyExistsAsync(formattedKey);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            Log.Error(ex, "Error checking existence in cache for key: {Key}", key);
            return false;
        }
    }

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<Task<T>> factory,
        TimeSpan? expiry = null,
        CancellationToken cancellationToken = default
    )
    {
        var cachedValue = await GetAsync<T>(key, cancellationToken);

        if (!EqualityComparer<T>.Default.Equals(cachedValue, default))
        {
            return cachedValue!;
        }

        Log.Debug("Cache miss for key: {Key}, creating new value", key);
        var newValue = await factory();

        if (!EqualityComparer<T>.Default.Equals(newValue, default))
        {
            await SetAsync(key, newValue, expiry, cancellationToken);
        }

        return newValue;
    }
}
