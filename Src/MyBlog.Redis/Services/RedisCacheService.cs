using MyBlog.Core.Services.Cache;
using MyBlog.Redis.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using StackExchange.Redis;

namespace MyBlog.Redis.Services;

/// <summary>
/// Redis implementation of the cache service
/// </summary>
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
#pragma warning disable CS0618
        JsonConvert.DefaultSettings = () =>
            new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    DefaultMembersSearchFlags =
                        System.Reflection.BindingFlags.Public
                        | System.Reflection.BindingFlags.NonPublic
                        | System.Reflection.BindingFlags.Instance,
                },
            };
#pragma warning restore CS0618

        _connectionProvider =
            connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
        _keyProvider = keyProvider ?? throw new ArgumentNullException(nameof(keyProvider));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        Log.Debug(
            "RedisCacheService initialized with connectionProvider: {ConnectionProvider}, keyProvider: {KeyProvider}, settings: {Settings}",
            connectionProvider.GetType().Name,
            keyProvider.GetType().Name,
            settings.GetType().Name
        );
    }

    public async Task<T?> GetAsync<T>(string[] key, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var formattedKey = _keyProvider.GenerateKey(
                new[] { typeof(T).Name }.Concat(key).ToArray()
            );
            var database = _connectionProvider.GetDatabase();
            var value = await database.StringGetAsync(formattedKey);

            if (value.IsNull)
            {
                Log.Debug("Cache miss for key: {Key}", formattedKey);
                return default;
            }

            Log.Debug("Cache hit for key: {Key}", formattedKey);
            return JsonConvert.DeserializeObject<T>(value.ToString());
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            Log.Error(ex, "Error getting value from cache for key: {Key}", key);
            return default;
        }
    }

    public async Task<bool> SetAsync<T>(
        string[] key,
        T value,
        TimeSpan? expiry = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var formattedKey = _keyProvider.GenerateKey(
                new[] { typeof(T).Name }.Concat(key).ToArray()
            );
            var database = _connectionProvider.GetDatabase();
            var serializedValue = JsonConvert.SerializeObject(value);

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

    public async Task<bool> RemoveAsync<T>(
        string[] key,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var formattedKey = _keyProvider.GenerateKey(
                new[] { typeof(T).Name }.Concat(key).ToArray()
            );
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

    public async Task<bool> ExistsAsync(string[] key, CancellationToken cancellationToken = default)
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
        string[] key,
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

    public async Task<T?> GetAndRemoveAsync<T>(
        string[] key,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var formattedKey = _keyProvider.GenerateKey(
                new[] { typeof(T).Name }.Concat(key).ToArray()
            );
            var database = _connectionProvider.GetDatabase();
            var value = await database.StringGetAsync(formattedKey);

            if (value.IsNull)
            {
                Log.Debug("Cache miss for key: {Key}", formattedKey);
                return default;
            }

            await database.KeyDeleteAsync(formattedKey);

            Log.Debug("Cache hit and removed for key: {Key}", formattedKey);
            Log.Debug("Fetched value: {Value}", value.ToString());
            return JsonConvert.DeserializeObject<T>(value.ToString());
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            Log.Error(ex, "Error getting and removing value from cache for key: {Key}", key);
            return default;
        }
    }

    public async Task<IEnumerable<T>> RemoveAllByKeyPatternAsync<T>(
        string pattern,
        bool isReturn = false,
        CancellationToken cancellationToken = default
    )
    {
        var removedValues = new List<T>();
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var server = _connectionProvider.GetServer();
            var database = _connectionProvider.GetDatabase();

            var keyLength = 0;
            do
            {
                var keys = server.Keys(database.Database, pattern).ToArray();
                keyLength = keys.Length;
                if (keyLength == 0)
                {
                    Log.Debug("No keys found matching pattern: {Pattern}", pattern);
                    return removedValues;
                }

                var deleteTasks = keys.Select(async key =>
                {
                    RedisValue? value = null;
                    if (isReturn)
                    {
                        value = await database.StringGetAsync(key);
                    }
                    bool deleted = await database.KeyDeleteAsync(key);

                    if (deleted && value.HasValue)
                    {
                        try
                        {
                            var deserialized = JsonConvert.DeserializeObject<T>(value!);
                            if (
                                !EqualityComparer<T>.Default.Equals(deserialized, default)
                                && deserialized != null
                            )
                            {
                                removedValues.Add(deserialized);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Warning(ex, "Failed to deserialize value for key: {Key}", key);
                        }
                    }
                });
                await Task.WhenAll(deleteTasks);

                Log.Debug("Removed {Count} keys matching pattern: {Pattern}", keys.Length, pattern);
            } while (keyLength > 0 && !cancellationToken.IsCancellationRequested);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            Log.Error(ex, "Error removing keys matching pattern: {Pattern}", pattern);
        }
        return removedValues;
    }
}
