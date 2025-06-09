using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBlog.Core.Aggregates.Blogs;
using MyBlog.Core.Aggregates.Users;
using MyBlog.Core.Primitives;

namespace MyBlog.Postgres.Data.TableConfigurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("comments");
        builder.HasKey(c => c.Id);

        builder
            .Property(c => c.Id)
            .ValueGeneratedNever()
            .HasColumnName("id")
            .HasColumnType("uuid")
            .HasConversion(id => id.Value, value => BaseId.From(value));

        builder
            .Property(comment => comment.BlogId)
            .HasColumnName("blog_id")
            .HasConversion(id => id.Value, value => BlogId.From(value));
        builder.Property(comment => comment.Content).HasColumnName("content").IsRequired();
        builder
            .Property(comment => comment.AuthorId)
            .HasColumnName("author_id")
            .IsRequired()
            .HasConversion(id => id.Value, value => UserId.From(value));

        builder.Property(e => e.CreatedAt).IsRequired().HasColumnName("created_at");

        builder.Property(e => e.UpdatedAt).IsRequired().HasColumnName("updated_at");

        builder.Property(e => e.DeletedAt).IsRequired(false).HasColumnName("deleted_at");
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted");
        builder.Ignore(e => e.ChildrenComment);
        builder
            .HasMany(c => c.ChildrenComment)
            .WithOne()
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Property(comment => comment.ParentCommentId)
            .HasColumnName("parent_comment_id")
#pragma warning disable CS8602
            .HasConversion(id => id.Value, value => BaseId.From(value))
#pragma warning restore CS8602
            .IsRequired(false);
    }
}
