using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using MyBlog.Core.Primitives;
using MyBlog.Core.Repositories;
using MyBlog.Core.Repositories.Models;

namespace MyBlog.Infrastructure.Data;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly MyBlogContext _context;
    private readonly IServiceProvider _provider;
    private readonly Dictionary<(Type, Type), object> _repositories;
    private readonly HashSet<IServiceScope> _scopes;
    private readonly Dictionary<string, IDbContextTransaction> _transactions;
    private bool _disposed;

    public UnitOfWork(MyBlogContext context, IServiceProvider provider)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _repositories = new Dictionary<(Type, Type), object>();
        _transactions = new Dictionary<string, IDbContextTransaction>();
        _scopes = new();
        _provider = provider;
    }

    public async Task CommitAtTransactionAsync(
        TransactionInformation transactionInformation,
        CancellationToken cancellationToken = default
    )
    {
        if (transactionInformation == null)
        {
            throw new ArgumentNullException(nameof(transactionInformation));
        }

        ThrowIfDisposed();

        if (!_transactions.TryGetValue(transactionInformation.Id, out var transaction))
        {
            throw new InvalidOperationException(
                $"Transaction with ID {transactionInformation.Id} not found."
            );
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        foreach (var transaction in _transactions.Values)
        {
            transaction.Dispose();
        }

        foreach (var scope in _scopes)
        {
            scope.Dispose();
        }

        _scopes.Clear();
        _transactions.Clear();
        _context.Dispose();
        _disposed = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        foreach (var transaction in _transactions.Values)
        {
            await transaction.DisposeAsync();
        }

        foreach (var scope in _scopes)
        {
            scope.Dispose();
        }

        _transactions.Clear();
        await _context.DisposeAsync();
        _disposed = true;
    }

    public IRepository<T, TId> Repository<T, TId>()
        where TId : notnull
        where T : Entity<TId>
    {
        ThrowIfDisposed();

        var entityType = typeof(T);
        var keyType = typeof(TId);
        if (!_repositories.ContainsKey((entityType, keyType)))
        {
            var scope = _provider.CreateScope();
            _scopes.Add(scope);
            _repositories[(entityType, keyType)] = scope.ServiceProvider.GetRequiredService<
                IRepository<T, TId>
            >();
        }

        return (IRepository<T, TId>)_repositories[(entityType, keyType)];
    }

    public async Task RollBackAtTransactionAsync(
        TransactionInformation transactionInformation,
        CancellationToken cancellationToken = default
    )
    {
        if (transactionInformation == null)
        {
            throw new ArgumentNullException(nameof(transactionInformation));
        }

        ThrowIfDisposed();

        if (!_transactions.TryGetValue(transactionInformation.Id, out var transaction))
        {
            throw new InvalidOperationException(
                $"Transaction with ID {transactionInformation.Id} not found."
            );
        }

        await transaction.RollbackAsync(cancellationToken);
        await transaction.DisposeAsync();
        _transactions.Remove(transactionInformation.Id);
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<TransactionInformation> StartTransactionAsync(
        string? transactionName = null,
        CancellationToken cancellationToken = default
    )
    {
        ThrowIfDisposed();

        var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        var transactionInfo = new TransactionInformation
        {
            Id = transaction.TransactionId.ToString(),
            Name = transactionName ?? $"Transaction_{transaction.TransactionId}",
        };

        _transactions[transactionInfo.Id] = transaction;

        return transactionInfo;
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(UnitOfWork));
        }
    }
}
