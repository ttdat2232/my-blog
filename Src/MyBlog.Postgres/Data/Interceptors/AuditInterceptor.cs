using Microsoft.EntityFrameworkCore.Diagnostics;
using MyBlog.Core.Primitives;

namespace MyBlog.Postgres.Data.Interceptors;

public sealed class AuditInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        if (eventData.Context == null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        foreach (
            var entry in eventData
                .Context.ChangeTracker.Entries<Entity<object>>()
                .Where(e =>
                    e.State == Microsoft.EntityFrameworkCore.EntityState.Modified
                    || e.State == Microsoft.EntityFrameworkCore.EntityState.Added
                )
        )
        {
            bool isCreated = entry.State == Microsoft.EntityFrameworkCore.EntityState.Added;
            entry.Entity.Audit(isCreated, DateTime.UtcNow);
        }
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
