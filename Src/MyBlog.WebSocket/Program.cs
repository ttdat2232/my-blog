using Microsoft.AspNetCore.Authentication;
using MyBlog.Application;
using MyBlog.Core;
using MyBlog.Jwt;
using MyBlog.Postgres;
using MyBlog.RabbitMq;
using MyBlog.Redis;
using MyBlog.WebSocket.BackgroundServices;
using MyBlog.WebSocket.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMyBlogCore();

builder.Services.AddMyBlogWebSocket();
builder.Services.AddMyBlogJwt(builder.Configuration);
builder.Services.AddMyBlogRedis(builder.Configuration);
builder.Services.AddMyBlogDatabase(builder.Configuration);
builder.Services.AddMyBlogRabbitMq(builder.Configuration);

var scheme = "Bearer";
builder
    .Services.AddAuthentication()
    .AddScheme<AuthenticationSchemeOptions, MyBlogAuthenticationHandler>(scheme, null);

builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy(
        "Authenticated",
        policy =>
        {
            policy.AuthenticationSchemes.Add(scheme);
            policy.RequireAuthenticatedUser();
        }
    );
    opts.AddPolicy(
        "Admin",
        policy =>
        {
            policy.AuthenticationSchemes.Add(scheme);
            policy.RequireAuthenticatedUser();
            policy.RequireRole("admin");
        }
    );
    opts.AddPolicy(
        "User",
        policy =>
        {
            policy.AuthenticationSchemes.Add(scheme);
            policy.RequireAuthenticatedUser();
            policy.RequireRole("user");
        }
    );
});
builder.Services.AddControllers();
builder.Services.AddHostedService<WebSocketConsumer>();

var app = builder.Build();

app.UseMiddleware<StatictistMiddleware>();

app.UseWebSockets();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<WebSocketMiddleware>();

app.MapControllers();

app.Run();
