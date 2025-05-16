using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyBlog.Core.Services.Cache;
using MyBlog.Redis.Services;
using MyBlog.Redis.Settings;

namespace MyBlog.Redis;

public static class DependencyInjection
{
    public static IServiceCollection AddMyBlogRedis(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddSingleton<ICacheSettings>(provider => new DefaultCacheSettings(configuration));
        services.AddSingleton<RedisCacheConnectionProvider>();

        services.AddSingleton<ICacheKeyProvider, DefaultCacheKeyProvider>();

        services.AddSingleton<ICacheService, RedisCacheService>();

        return services;
    }
}
