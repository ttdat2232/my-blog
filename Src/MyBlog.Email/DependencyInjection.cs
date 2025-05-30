using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyBlog.Core.Services.Email;
using MyBlog.Email.Configurations;

namespace MyBlog.Email;

public static class DependencyInjection
{
    public static IServiceCollection AddMyBlogEmailServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<SmtpSettings>(configuration.GetSection("Smtp"));
        services.AddScoped<IEmailService, EmailService>();
        return services;
    }
}
