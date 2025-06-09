using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBlog.Core.Aggregates.Blogs;
using MyBlog.Core.Aggregates.Tags;

namespace MyBlog.Postgres.Data.TableConfigurations;

public class BlogTagConfiguration : IEntityTypeConfiguration<BlogTag>
{
    public void Configure(EntityTypeBuilder<BlogTag> builder)
    {
        builder.ToTable("blog_tags");
        builder.HasKey(e => new { e.BlogId, e.TagId });

        builder
            .Property(e => e.BlogId)
            .HasColumnName("blog_id")
            .HasConversion(key => key.Value, value => BlogId.From(value))
            .IsRequired();
        builder
            .Property(e => e.TagId)
            .HasColumnName("tag_id")
            .HasConversion(key => key.Value, value => TagId.From(value))
            .IsRequired();

        builder.Ignore(e => e.Id);
        builder.Ignore(e => e.CreatedAt);
        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedAt);
        builder.Ignore(e => e.UpdatedBy);
        builder.Ignore(e => e.DeletedAt);
        builder.Ignore(e => e.IsDeleted);

        builder
            .HasOne<BlogAggregate>()
            .WithMany()
            .HasForeignKey(b => b.BlogId)
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasOne<TagAggregate>()
            .WithMany()
            .HasForeignKey(t => t.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
