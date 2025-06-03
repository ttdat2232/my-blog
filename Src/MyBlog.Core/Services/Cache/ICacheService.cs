namespace MyBlog.Core.Services.Cache;

/// <summary>
/// Interface for cache operations
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Gets an item from the cache
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The cached item, or null if not found</returns>
    Task<T?> GetAsync<T>(string[] key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an item from the cache
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The cached item, or null if not found</returns>
    Task<T?> GetAndRemoveAsync<T>(string[] key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets an item in the cache with optional expiration
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <param name="value">Item to cache</param>
    /// <param name="expiry">Optional expiration timespan</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if set successfully</returns>
    Task<bool> SetAsync<T>(
        string[] key,
        T value,
        TimeSpan? expiry = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Removes an item from the cache
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <typeparam name="T">Target</typeparam>
    /// <returns></returns>
    Task<bool> RemoveAsync<T>(string[] key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an item exists in the cache
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the item exists</returns>
    Task<bool> ExistsAsync(string[] key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an item from cache or creates it if missing
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <param name="factory">Factory function to create the item if not in cache</param>
    /// <param name="expiry">Optional expiration timespan</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The cached or newly created item</returns>
    Task<T> GetOrCreateAsync<T>(
        string[] key,
        Func<Task<T>> factory,
        TimeSpan? expiry = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Remove items when their key match a pattern
    /// </summary>
    /// <param name="pattern">Key pattern</param>
    /// <param name="isReturn">Whether return removed values</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    Task<IEnumerable<T>> RemoveAllByKeyPatternAsync<T>(
        string pattern,
        bool isReturn = false,
        CancellationToken cancellationToken = default
    );
}
