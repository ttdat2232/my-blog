using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBlog.Core.Aggregates.Categories;
using MyBlog.Core.Aggregates.Users;

namespace MyBlog.Postgres.Data.TableConfigurations;

public class CategoryConfiguration : IEntityTypeConfiguration<CategoryAggregate>
{
    public void Configure(EntityTypeBuilder<CategoryAggregate> builder)
    {
        builder.ToTable("categories");
        builder.HasKey(e => e.Id);

        builder
            .Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => CategoryId.From(value));

        builder.Property(e => e.Name).HasColumnName("name").IsRequired();

        builder
            .Property(e => e.Description)
            .HasColumnName("description")
            .IsRequired(false)
            .HasColumnType("TEXT");

        builder.Property(e => e.NormalizeName).HasColumnName("normalize_name").IsRequired();

        builder.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").IsRequired();

        builder.Property(e => e.DeletedAt).HasColumnName("deleted_at").IsRequired(false);

        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").IsRequired();
    }
}
