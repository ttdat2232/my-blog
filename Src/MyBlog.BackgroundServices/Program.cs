using MyBlog.Application;
using MyBlog.BackgroundServices.Services;
using MyBlog.Core;
using MyBlog.Email;
using MyBlog.Jwt;
using MyBlog.Postgres;
using MyBlog.RabbitMq;
using MyBlog.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMyBlogCore();
builder.Services.AddMyBlogApplication();
builder.Services.AddMyBlogRedis(builder.Configuration);
builder.Services.AddMyBlogDatabase(builder.Configuration);
builder.Services.AddMyBlogEmailServices(builder.Configuration);
builder.Services.AddMyBlogJwt(builder.Configuration);
builder.Services.AddMyBlogRabbitMq(builder.Configuration);

builder.Services
// .AddHostedService<UpdateViewCountScheduler>()
// .AddHostedService<BlogCreatedEventHandler>()
.AddHostedService<BlogCommentAddedEventHandler>();
var app = builder.Build();

app.Run();
