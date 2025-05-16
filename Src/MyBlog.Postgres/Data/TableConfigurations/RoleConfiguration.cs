using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBlog.Core.Aggregates.Roles;
using MyBlog.Core.Aggregates.Users;

namespace MyBlog.Postgres.Data.TableConfigurations;

public class RoleConfiguration : IEntityTypeConfiguration<RoleAggregate>
{
    public void Configure(EntityTypeBuilder<RoleAggregate> builder)
    {
        builder.ToTable("roles");
        builder.HasKey(e => e.Id);

        builder
            .Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => RoleId.From(value));

        builder.Property(e => e.Name).HasColumnName("name").IsRequired();

        builder.Property(e => e.NormalizeName).HasColumnName("normalize_name").IsRequired();

        builder.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").IsRequired();

        builder.Property(e => e.DeletedAt).HasColumnName("deleted_at").IsRequired(false);

        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").IsRequired();

        builder.Property(e => e.CreatedBy).HasColumnName("created_by");
        builder.Property(e => e.UpdatedBy).HasColumnName("updated_by");

        builder
            .HasIndex(e => e.NormalizeName)
            .HasDatabaseName("ix_roles_normalize_name")
            .IsUnique();
    }
}
