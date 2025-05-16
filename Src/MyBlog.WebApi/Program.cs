using Carter;
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

builder.Services.AddHttpClient(
    "auth",
    conifg =>
    {
        conifg.BaseAddress = new Uri(
            builder.Configuration["Auth:Url"] ?? throw new ArgumentException("Auth:Url")
        );
        conifg.Timeout = TimeSpan.FromSeconds(30);
    }
);

builder
    .Services.AddAuthentication()
    .AddScheme<AuthenticationSchemeOptions, MyBlogAuthenticationHandler>("MyBlogBearer", null);

builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("User", config => { });
});

builder.Services.AddCarter();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapCarter();

await app.RunAsync();
