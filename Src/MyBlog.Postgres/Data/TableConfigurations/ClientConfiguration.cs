using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBlog.Core.Aggregates.Clients;

namespace MyBlog.Postgres.Data.TableConfigurations;

public class ClientConfiguration : IEntityTypeConfiguration<ClientAggregate>
{
    public void Configure(EntityTypeBuilder<ClientAggregate> builder)
    {
        builder.ToTable("clients");

        builder.HasKey(e => e.Id);
        builder
            .Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => ClientId.From(value));

        builder.Property(x => x.ClientSecret).HasColumnName("client_secret").IsRequired();

        builder
            .Property<List<string>>("_redirectUris")
            .HasColumnName("redirect_uris")
            .HasColumnType("text[]")
            .IsRequired();
        builder
            .Property<List<string>>("_allowScopes")
            .HasColumnName("allow_scopes")
            .HasColumnType("text[]")
            .IsRequired();

        builder.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(e => e.DeletedAt).HasColumnName("deleted_at").IsRequired(false);
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").IsRequired();
        builder.Property(e => e.CreatedBy).HasColumnName("created_by");
        builder.Property(e => e.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(e => e.ClientSecret).HasDatabaseName("ix_client_secret_unique").IsUnique();
    }
}
