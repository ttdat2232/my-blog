using MyBlog.Redis.Settings;
using Serilog;
using StackExchange.Redis;

namespace MyBlog.Redis.Services;

/// <summary>
/// Redis implementation of cache connection provider
/// </summary>
public class RedisCacheConnectionProvider : IDisposable
{
    private readonly ICacheSettings _settings;
    private ConnectionMultiplexer? _connection;
    private bool _disposed;

    public RedisCacheConnectionProvider(ICacheSettings settings)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        InitializeConnection();
    }

    private void InitializeConnection()
    {
        try
        {
            _connection = ConnectionMultiplexer.Connect(_settings.ConnectionString);
            Log.Information("Redis connection established successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to connect to Redis");
            _connection = null;
        }
    }

    public IDatabase GetDatabase()
    {
        if (_connection == null || !IsConnected())
        {
            InitializeConnection();

            if (_connection == null)
            {
                throw new InvalidOperationException("Cannot connect to Redis");
            }
        }

        return _connection.GetDatabase();
    }

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
