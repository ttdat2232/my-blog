using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBlog.Core.Aggregates.Blogs;
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
