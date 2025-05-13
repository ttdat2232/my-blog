using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBlog.Core.Aggregates.Categories;
using MyBlog.Core.Aggregates.Tags;
using MyBlog.Core.Primitives;

namespace MyBlog.Postgres.Data.TableConfigurations;

public class TagConfiguration : IEntityTypeConfiguration<TagAggregate>
{
    public void Configure(EntityTypeBuilder<TagAggregate> builder)
    {
        builder.ToTable("tags");
        builder.HasKey(e => e.Id).HasName("id");

        builder
            .Property(e => e.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => TagId.From(value))
            .HasColumnName("id");

        builder.Property(e => e.CreatedAt).IsRequired().HasColumnName("created_at");

        builder.Property(e => e.UpdatedAt).IsRequired().HasColumnName("updated_at");

        builder.Property(e => e.DeletedAt).IsRequired(false).HasColumnName("deleted_at");
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted");

        builder.Property(e => e.Name).IsRequired().HasColumnName("name");
    }
}
