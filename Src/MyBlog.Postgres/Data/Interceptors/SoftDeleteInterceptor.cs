using Microsoft.EntityFrameworkCore.Diagnostics;
using MyBlog.Core.Primitives;

namespace MyBlog.Postgres.Data.Interceptors;

public sealed class SoftDeleteInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        if (eventData.Context == null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        var markDeletedEntries = eventData
            .Context.ChangeTracker.Entries<Entity<object>>()
            .Where(e => e.State == Microsoft.EntityFrameworkCore.EntityState.Deleted);
        foreach (var entry in markDeletedEntries)
        {
            entry.Entity.Delete();
            entry.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
