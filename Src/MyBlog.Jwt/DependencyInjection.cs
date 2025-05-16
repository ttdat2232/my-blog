using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyBlog.Core.Repositories;
using MyBlog.Core.Services.Auth;
using MyBlog.Jwt.Configurations;
using MyBlog.Jwt.Repositories;
using MyBlog.Jwt.Services;

namespace MyBlog.Jwt;

public static class DependencyInjection
{
    public static IServiceCollection AddMyBlogJwt(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.AddScoped<ITokenRepository, TokenRepository>();
        services.AddScoped<ITokenService, JwtService>();
        return services;
    }
}
