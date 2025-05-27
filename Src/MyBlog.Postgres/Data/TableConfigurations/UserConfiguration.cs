using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBlog.Core.Aggregates.Roles;
using MyBlog.Core.Aggregates.Users;

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

        builder.Ignore(e => e.Roles);

        // Configure many-to-many relationship with roles using join entity
        builder
            .HasMany<RoleAggregate>()
            .WithMany()
            .UsingEntity<UserRole>(
                right =>
                    right
                        .HasOne<RoleAggregate>()
                        .WithMany()
                        .HasForeignKey(e => e.RoleId)
                        .OnDelete(DeleteBehavior.Cascade),
                left =>
                    left.HasOne<UserAggregate>()
                        .WithMany("_roles")
                        .HasForeignKey(e => e.UserId)
                        .OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.ToTable("user_roles");
                    join.HasKey(ur => new { ur.UserId, ur.RoleId });

                    join.Property(ur => ur.UserId)
                        .HasColumnName("user_id")
                        .HasConversion(id => id.Value, value => UserId.From(value));

                    join.Property(ur => ur.RoleId)
                        .HasColumnName("role_id")
                        .HasConversion(id => id.Value, value => RoleId.From(value));

                    join.Ignore(e => e.Id);
                    join.Ignore(e => e.CreatedAt);
                    join.Ignore(e => e.UpdatedAt);
                    join.Ignore(e => e.DeletedAt);
                    join.Ignore(e => e.CreatedBy);
                    join.Ignore(e => e.UpdatedBy);
                    join.Ignore(e => e.IsDeleted);
                }
            );

        builder
            .HasIndex(x => x.NormalizeUserName)
            .HasDatabaseName("ix_users_normalize_user_name")
            .IsUnique();

        builder
            .HasIndex(x => x.NormalizeEmail)
            .HasDatabaseName("ix_users_normalize_email")
            .IsUnique();

        builder
            .Metadata.FindNavigation(nameof(UserAggregate.Roles))
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder
            .Metadata.FindNavigation(nameof(UserAggregate.Follows))
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);
        builder
            .Metadata.FindNavigation(nameof(UserAggregate.FollowedBy))
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
