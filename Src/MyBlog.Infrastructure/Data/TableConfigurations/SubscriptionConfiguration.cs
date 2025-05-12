using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBlog.Core.Aggregates.Users;
using MyBlog.Core.Primitives;

namespace MyBlog.Infrastructure.Data.TableConfigurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("subscriptions");

        builder.HasKey(e => new { e.FollowerId, e.FollowedId });

        builder
            .Property(e => e.FollowerId)
            .HasColumnName("follower_id")
            .HasConversion(id => id.Value, value => UserId.From(value));
        builder
            .Property(e => e.FollowedId)
            .HasColumnName("followed_id")
            .HasConversion(id => id.Value, value => UserId.From(value));

        builder
            .Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValue(DateTime.UtcNow);

        builder
            .Property(e => e.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValue(DateTime.UtcNow);

        builder.HasOne<UserAggregate>().WithMany(e => e.Follows).HasForeignKey(e => e.FollowerId);

        builder
            .HasOne<UserAggregate>()
            .WithMany(e => e.FollowedBy)
            .HasForeignKey(e => e.FollowedId);

        builder.Ignore(e => e.Id);
        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
        builder.Ignore(e => e.DeletedAt);
        builder.Ignore(e => e.IsDeleted);
    }
}
