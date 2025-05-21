using MyBlog.Redis.Settings;
using Serilog;
using StackExchange.Redis;

namespace MyBlog.Redis.Services;

/// <summary>
/// Redis implementation of cache connection provider
/// </summary>
public class RedisCacheConnectionProvider : IDisposable
{
    private readonly IConnectionMultiplexer _connection;
    private bool _disposed;

    public RedisCacheConnectionProvider(IConnectionMultiplexer connectionMultiplexer)
    {
        _connection = connectionMultiplexer;
    }

    public IDatabase GetDatabase()
    {
        return _connection.GetDatabase();
    }

    public IServer GetServer() => _connection.GetServer(_connection.GetEndPoints()[0]);

    public bool IsConnected()
    {
        return _connection?.IsConnected ?? false;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            _connection?.Dispose();
        }

        _disposed = true;
    }
}
