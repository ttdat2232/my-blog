using Microsoft.Extensions.DependencyInjection;
using MyBlog.Core.Services.Blogs;
using MyBlog.Core.Services.Cache;

namespace MyBlog.BackgroundServices.Services;

public sealed class UpdateViewCountScheduler(IServiceProvider _serviceProvider)
    : IHostedService,
        IDisposable
{
    private Timer? _timer;
    private bool _isRunning;
    private readonly object _lock = new();

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Serilog.Log.Debug("UpdateViewCountScheduler starting...");
        _timer = new Timer(
            (state) => DoWork(state, cancellationToken),
            null,
            TimeSpan.Zero,
            TimeSpan.FromMinutes(5)
        );
#if DEBUG
        _timer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(10));
        Serilog.Log.Debug("Running in DEBUG mode with 10 second intervals");
#endif
        return Task.CompletedTask;
    }

    private async void DoWork(object? state, CancellationToken cancellationToken)
    {
        if (_isRunning)
        {
            Serilog.Log.Debug("Skipping view count update - previous operation still running");
            return;
        }

        lock (_lock)
        {
            if (_isRunning)
            {
                return;
            }
            _isRunning = true;
            Serilog.Log.Debug("Starting view count update operation");
        }

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
            var blogService = scope.ServiceProvider.GetRequiredService<IBlogService>();

            Serilog.Log.Debug("Fetching view counts from cache");
            var viewCounts = await cacheService.GetAndRemoveAsync<IDictionary<Guid, long>>(
                "blog:viewcount",
                cancellationToken
            );

            if (viewCounts != null && viewCounts.Count > 0)
            {
                Serilog.Log.Debug(
                    "Retrieved {count} view counts from cache, updating blog service",
                    viewCounts.Count
                );
                await blogService.UpdateViewCount(viewCounts, cancellationToken);
                Serilog.Log.Information(
                    "Successfully updated view counts for {count} blogs",
                    viewCounts.Count
                );
            }
            else
            {
                Serilog.Log.Debug("No view counts found in cache to update");
            }
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "Error occurred while updating blog view counts");
        }
        finally
        {
            _isRunning = false;
            Serilog.Log.Debug("View count update operation completed");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Serilog.Log.Debug("UpdateViewCountScheduler stopping...");
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
        GC.SuppressFinalize(this);
    }
}
