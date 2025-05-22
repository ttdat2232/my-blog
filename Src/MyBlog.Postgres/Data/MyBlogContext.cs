using Microsoft.EntityFrameworkCore;
using MyBlog.Core.Aggregates.Blogs;
using MyBlog.Core.Primitives;

namespace MyBlog.Postgres.Data;

public class MyBlogContext : DbContext
{
    public MyBlogContext(DbContextOptions<MyBlogContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<BaseId>();
        modelBuilder.Ignore<DomainEvent>();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyBlogContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
