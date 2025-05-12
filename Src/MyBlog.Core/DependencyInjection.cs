using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyBlog.Core.Services.Blogs;
using Serilog;

namespace MyBlog.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddMyBlogCore(this IServiceCollection services)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        services.AddScoped<IBlogService, BlogService>();
        return services;
    }
}
