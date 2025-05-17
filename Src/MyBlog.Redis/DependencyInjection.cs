using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyBlog.Core.Services.Cache;
using MyBlog.Redis.Services;
using MyBlog.Redis.Settings;
using StackExchange.Redis;

namespace MyBlog.Redis;

public static class DependencyInjection
{
    public static IServiceCollection AddMyBlogRedis(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddSingleton<ICacheSettings>(provider => new DefaultCacheSettings(configuration));
        services.AddSingleton<ICacheKeyProvider, DefaultCacheKeyProvider>();
        services.AddSingleton<ICacheService, RedisCacheService>();
        var redisConnectionString =
            configuration["Redis:ConnectionString"]
            ?? throw new InvalidOperationException("Redis connection string is not configured");
        IConnectionMultiplexer connection = ConnectionMultiplexer.Connect(redisConnectionString);
        Serilog.Log.Information("Redis connection established successfully");

        services.AddSingleton(_ => connection);
        services.AddSingleton<RedisCacheConnectionProvider>();
        services.AddStackExchangeRedisCache(config =>
        {
            config.ConnectionMultiplexerFactory = () => Task.FromResult(connection);
        });
        return services;
    }
}
