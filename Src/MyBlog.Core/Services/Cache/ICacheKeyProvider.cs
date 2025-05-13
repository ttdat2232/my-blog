namespace MyBlog.Core.Services.Cache;

/// <summary>
/// Interface for cache key management
/// </summary>
public interface ICacheKeyProvider
{
    /// <summary>
    /// Generates a cache key based on provided parameters
    /// </summary>
    /// <param name="keyParts">Parts to include in the key</param>
    /// <returns>Formatted cache key</returns>
    string GenerateKey(params string[] keyParts);
}
