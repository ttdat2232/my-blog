using Carter;
using MyBlog.Application;
using MyBlog.Core;
using MyBlog.Infrastructure;
using MyBlog.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddMyBlogCore();
builder.Services.AddMyBlogApplication();
builder.Services.AddMyBlogJwt(builder.Configuration);
builder.Services.AddMyBlogDatabase(builder.Configuration);

builder.Services.AddCarter();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapCarter();

app.Run();
