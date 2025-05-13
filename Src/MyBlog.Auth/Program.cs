using Carter;
using MyBlog.Application;
using MyBlog.Core;
using MyBlog.Infrastructure;
using MyBlog.Jwt;
using MyBlog.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMyBlogCore();
builder.Services.AddMyBlogApplication();
builder.Services.AddMyBlogJwt(builder.Configuration);
builder.Services.AddMyBlogDatabase(builder.Configuration);
builder.Services.AddMyBlogRedis(builder.Configuration);

builder.Services.AddOpenApi();

builder.Services.AddCarter();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapCarter();
app.Run();
