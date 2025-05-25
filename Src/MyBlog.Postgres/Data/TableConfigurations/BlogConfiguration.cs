using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBlog.Core.Aggregates.Blogs;
using MyBlog.Core.Aggregates.Categories;
using MyBlog.Core.Aggregates.Users;
using MyBlog.Core.Primitives;

namespace MyBlog.Postgres.Data.TableConfigurations;

public class BlogConfiguration : IEntityTypeConfiguration<BlogAggregate>
{
    public void Configure(EntityTypeBuilder<BlogAggregate> builder)
    {
        builder.ToTable("blogs");
        builder.HasKey(e => e.Id);

        builder
            .Property(e => e.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => BlogId.From(value));

        builder
            .Property(e => e.Id)
            .ValueGeneratedNever()
            .HasColumnName("id")
            .HasConversion(id => id.Value, value => BlogId.From(value));

        builder.Property(blog => blog.Title).HasColumnName("title").IsRequired().HasMaxLength(200);

        builder
            .Property(blog => blog.Content)
            .HasColumnName("content")
            .HasColumnType("TEXT")
            .IsRequired();

        builder
            .Property(blog => blog.ViewCount)
            .HasColumnName("view_count")
            .IsRequired()
            .HasDefaultValue(0);

        builder
            .Property(blog => blog.Status)
            .HasColumnName("status")
            .IsRequired()
            .HasConversion<string>();

        builder
            .Property(blog => blog.IsPublished)
            .HasColumnName("is_published")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.CreatedAt).IsRequired().HasColumnName("created_at");

        builder.Property(e => e.UpdatedAt).IsRequired().HasColumnName("updated_at");

        builder.Property(e => e.DeletedAt).IsRequired(false).HasColumnName("deleted_at");
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted");

        builder.Property(blog => blog.PublishDate).HasColumnName("publish_date").IsRequired(false);

        builder
            .Property(blog => blog.CategoryId)
            .HasColumnName("category_id")
            .HasConversion(id => id.Value, value => CategoryId.From(value))
            .IsRequired(false);

        builder
            .Property(blog => blog.AuthorId)
            .HasColumnName("author_id")
            .HasConversion(id => id.Value, value => UserId.From(value))
            .IsRequired(false);

        builder
            .HasOne<CategoryAggregate>()
            .WithMany()
            .HasForeignKey(e => e.CategoryId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne<UserAggregate>()
            .WithMany()
            .HasForeignKey(e => e.AuthorId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        ConfigureComnentRelation(builder);

        builder
            .Metadata.FindNavigation(nameof(BlogAggregate.Tags))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
        builder
            .Metadata.FindNavigation(nameof(BlogAggregate.Comments))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }

    private static void ConfigureComnentRelation(EntityTypeBuilder<BlogAggregate> builder)
    {
        builder.HasMany(c => c.Comments).WithOne().HasForeignKey(c => c.BlogId);
    }
}
