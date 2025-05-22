using MyBlog.Application;
using MyBlog.BackgroundServices.Services;
using MyBlog.Core;
using MyBlog.Postgres;
using MyBlog.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMyBlogCore();
builder.Services.AddMyBlogApplication();
builder.Services.AddMyBlogRedis(builder.Configuration);
builder.Services.AddMyBlogDatabase(builder.Configuration);

builder.Services.AddHostedService<UpdateViewCountScheduler>();
var app = builder.Build();

app.Run();
