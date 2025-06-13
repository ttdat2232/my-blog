using Microsoft.Extensions.DependencyInjection;
using MyBlog.Application.Services;
using MyBlog.Core.Services.Socket;

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

    public static IServiceCollection AddMyBlogWebSocket(this IServiceCollection services)
    {
        services.AddSingleton<IWebSocketManager, WebSocketManager>();
        services.AddScoped<IMessageProcessor, MessageProccessor>();
        services.AddScoped<IWebSocketHandler, WebSocketHandler>();
        return services;
    }
}
