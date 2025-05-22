using System.Reflection;
using Microsoft.EntityFrameworkCore.Storage;
using MyBlog.Core.Primitives;
using MyBlog.Core.Repositories;
using MyBlog.Core.Repositories.Models;
using MyBlog.Postgres.Data.Repositories;

namespace MyBlog.Postgres.Data;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly MyBlogContext _context;
    private readonly Dictionary<(Type, Type), object> _repositories;
    private readonly Dictionary<string, IDbContextTransaction> _transactions;
    private bool _disposed;
    private static readonly Dictionary<Type, ConstructorInfo> _constructorCache = new();

    public UnitOfWork(MyBlogContext context, IServiceProvider provider)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _repositories = new Dictionary<(Type, Type), object>();
        _transactions = new Dictionary<string, IDbContextTransaction>();
    }

    public IBlogRepository BlogRepository => new CustomBlogRepository(_context);

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

        if (!_transactions.ContainsKey(transactionInformation.Id))
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

        _transactions.Clear();
        _repositories.Clear();
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

        _transactions.Clear();
        _repositories.Clear();
        await _context.DisposeAsync();
        _disposed = true;
    }

    public IRepository<T, TId> Repository<T, TId>()
        where TId : BaseId
        where T : Entity<TId>
    {
        ThrowIfDisposed();

        var key = (typeof(T), typeof(TId));
        if (_repositories.TryGetValue(key, out var repository))
        {
            return (IRepository<T, TId>)repository;
        }

        var repositoryType = typeof(Repository<,>).MakeGenericType(typeof(T), typeof(TId));
        if (!_constructorCache.TryGetValue(repositoryType, out var constructor))
        {
            constructor = repositoryType.GetConstructor(new[] { typeof(MyBlogContext) });
            if (constructor == null)
            {
                throw new InvalidOperationException(
                    $"No suitable constructor found for {repositoryType}"
                );
            }
            _constructorCache[repositoryType] = constructor;
        }

        _repositories[key] = constructor.Invoke(new object[] { _context });
        return (IRepository<T, TId>)_repositories[key];
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

    public async Task<bool> SaveAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        return await _context.SaveChangesAsync(cancellationToken) > 0;
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
