using Microsoft.Extensions.DependencyInjection;

namespace MyBlog.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddMyBlogApplication(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(typeof(DependencyInjection).Assembly);
        });
        return services;
    }
}
