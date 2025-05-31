using Carter;
using MyBlog.Application;
using MyBlog.Core;
using MyBlog.Jwt;
using MyBlog.Postgres;
using MyBlog.RabbitMq;
using MyBlog.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMyBlogCore();
builder.Services.AddMyBlogApplication();
builder.Services.AddMyBlogJwt(builder.Configuration);
builder.Services.AddMyBlogDatabase(builder.Configuration);
builder.Services.AddMyBlogRedis(builder.Configuration);
builder.Services.AddMyBlogRabbitMq(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddCarter();
builder.Services.AddSession();
builder.Services.AddRazorPages();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseSession();
app.MapCarter();
app.UseStaticFiles();
app.MapControllers();
app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}",
    new { controller = "Home", action = "Index" }
);
app.MapRazorPages();

await app.RunAsync();
