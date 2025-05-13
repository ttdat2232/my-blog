using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBlog.Core.Aggregates.Users;
using MyBlog.Core.Primitives;

namespace MyBlog.Postgres.Data.TableConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<UserAggregate>
{
    public void Configure(EntityTypeBuilder<UserAggregate> builder)
    {
        builder.ToTable("users");

        // Configure primary key
        builder.HasKey(x => x.Id);
        builder
            .Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => UserId.From(value));

        // Configure properties
        builder
            .Property(x => x.UserName)
            .HasColumnName("user_name")
            .IsRequired()
            .HasMaxLength(100);

        builder
            .Property(x => x.NormalizeUserName)
            .HasColumnName("normalize_user_name")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Email).HasColumnName("email").IsRequired().HasMaxLength(256);

        builder
            .Property(x => x.NormalizeEmail)
            .HasColumnName("normalize_email")
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.Password).HasColumnName("password").IsRequired();

        builder.Property(x => x.Avatar).HasColumnName("avatar").IsRequired(false);

        builder.Property(e => e.CreatedAt).IsRequired().HasColumnName("created_at");

        builder.Property(e => e.UpdatedAt).IsRequired().HasColumnName("updated_at");

        builder.Property(e => e.DeletedAt).IsRequired(false).HasColumnName("deleted_at");

        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted");

        // Configure indexes
        builder
            .HasIndex(x => x.NormalizeUserName)
            .HasDatabaseName("ix_users_normalize_user_name")
            .IsUnique();

        builder
            .HasIndex(x => x.NormalizeEmail)
            .HasDatabaseName("ix_users_normalize_email")
            .IsUnique();

        builder
            .Metadata.FindNavigation("_follows")
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);
        builder
            .Metadata.FindNavigation("_followedBy")
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
