using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyBlog.Core.Repositories;
using MyBlog.Postgres.Configurations;
using MyBlog.Postgres.Data;
using Serilog;

namespace MyBlog.Postgres;

public static class DependencyInjection
{
    public static IServiceCollection AddMyBlogDatabase(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var databaseConfig = new DatabaseConfiguration();
        configuration.GetSection("Database").Bind(databaseConfig);
        services.Configure<DatabaseConfiguration>(config =>
        {
            config.TimeOutInMs = databaseConfig.TimeOutInMs;
        });

        var interceptors = typeof(DependencyInjection)
            .Assembly.GetTypes()
            .Where(t =>
                t.IsClass && !t.IsAbstract && t.IsAssignableTo(typeof(ISaveChangesInterceptor))
            )
            .ToList();
        foreach (var interceptor in interceptors)
        {
            services.AddScoped(interceptor);
        }

        var connectionString = configuration.GetConnectionString("Default") ?? "";
        services.AddDbContext<MyBlogContext>(
            (sp, optionsBuilder) =>
            {
                optionsBuilder.AddInterceptors(
                    interceptors.Select(interceptor =>
                        (IInterceptor)sp.GetRequiredService(interceptor)
                    )
                );
                optionsBuilder.ConfigureWarnings(warnings =>
                    warnings.Ignore(RelationalEventId.PendingModelChangesWarning)
                );
                optionsBuilder.UseNpgsql(
                    connectionString,
                    databaseOpts =>
                    {
                        databaseOpts.EnableRetryOnFailure();
                        databaseOpts.CommandTimeout(databaseConfig.TimeOutInMs);
                    }
                );
                optionsBuilder.LogTo(
                    Log.Information,
                    Microsoft.Extensions.Logging.LogLevel.Information
                );
#if DEBUG
                optionsBuilder.EnableSensitiveDataLogging();
#endif
            }
        );

        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
