using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MyBlog.Core.Services.Messages;
using MyBlog.RabbitMq.Configurations;

namespace MyBlog.RabbitMq;

public static class DependencyInjection
{
    public static IServiceCollection AddMyBlogRabbitMq(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<RabbitMqConfiguration>(configuration.GetSection("MessageBroker"));
        services.AddSingleton<IMessageBroker>(propvider =>
        {
            var messageBrokerType = configuration["MessageBroker:Type"]!;

            var configOpts = propvider.GetRequiredService<IOptions<RabbitMqConfiguration>>();
            var serializer = propvider.GetRequiredService<IMessageSerializer>();
            return messageBrokerType.ToLower() switch
            {
                "rabbitmq" => new RabbitMqMessageBroker(serializer, configOpts),
                _ => throw new ArgumentException($"Not support {messageBrokerType} at the moment"),
            };
        });
        return services;
    }
}
