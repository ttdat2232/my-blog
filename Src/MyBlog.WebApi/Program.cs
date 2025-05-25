using Microsoft.AspNetCore.Authentication;
using MyBlog.Application;
using MyBlog.Core;
using MyBlog.Jwt;
using MyBlog.Postgres;
using MyBlog.Redis;
using MyBlog.WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddMyBlogCore();
builder.Services.AddMyBlogApplication();
builder.Services.AddMyBlogJwt(builder.Configuration);
builder.Services.AddMyBlogRedis(builder.Configuration);
builder.Services.AddMyBlogDatabase(builder.Configuration);

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
            policy.RequireClaim("role", "admin");
        }
    );
    opts.AddPolicy(
        "User",
        policy =>
        {
            policy.AuthenticationSchemes.Add(scheme);
            policy.RequireAuthenticatedUser();
            policy.RequireRole("user");
            policy.RequireClaim("role", "user");
        }
    );
});

builder.Services.AddControllers();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
