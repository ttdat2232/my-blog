using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyBlog.Core.Services.Blogs;
using Serilog;

namespace MyBlog.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddMyBlogCore(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
        });

        services.AddScoped<IBlogService, BlogService>();
        return services;
    }
}
