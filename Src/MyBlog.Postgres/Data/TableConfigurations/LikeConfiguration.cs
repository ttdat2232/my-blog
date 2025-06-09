// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
// using MyBlog.Core.Aggregates.Blogs;
// using MyBlog.Core.Aggregates.Users;
// using MyBlog.Postgres.Data.ValueGenerators;

// namespace MyBlog.Postgres.Data.TableConfigurations;

// public sealed class LikeConfiguration : IEntityTypeConfiguration<Like>
// {
//     public void Configure(EntityTypeBuilder<Like> builder)
//     {
//         var blogIdColName = "blog_id";
//         var userIdColName = "user_id";
//         builder.ToTable("likes");
//         builder
//             .Property<BlogId>(blogIdColName)
//             .IsRequired()
//             .HasConversion(id => id.Value, value => BlogId.From(value));
//         builder
//             .Property<UserId>(userIdColName)
//             .IsRequired()
//             .HasConversion(id => id.Value, value => UserId.From(value));
//         builder.Property(blogIdColName).HasValueGenerator<LikeBlogIdValueGenerator>();
//         builder.Property(userIdColName).HasValueGenerator<LikeUserIdValueGenerator>();
//         builder.HasKey(blogIdColName, userIdColName);

//         builder.HasOne<BlogAggregate>().WithMany(b => b.Likes).HasForeignKey(blogIdColName);
//         builder.HasOne<UserAggregate>().WithMany().HasForeignKey(userIdColName);

//         builder.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();

//         builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").IsRequired();

//         builder.Property(e => e.DeletedAt).HasColumnName("deleted_at").IsRequired(false);

//         builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").IsRequired();

//         builder.Ignore(l => l.Id);
//         builder.Ignore(l => l.CreatedBy);
//         builder.Ignore(l => l.UpdatedBy);
//         builder.Ignore(l => l.DomainEvents);
//     }
// }
