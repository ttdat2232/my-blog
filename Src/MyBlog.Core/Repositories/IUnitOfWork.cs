using MyBlog.Core.Primitives;
using MyBlog.Core.Repositories.Models;

namespace MyBlog.Core.Repositories;

/// <summary>
/// Represents a unit of work for managing transactions and repositories
/// </summary>
public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Gets the repository for the specified entity type
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <returns>The repository instance</returns>
    IRepository<T, TId> Repository<T, TId>()
        where TId : BaseId
        where T : Entity<TId>;

    IBlogRepository BlogRepository { get; }

    /// <summary>
    /// Saves all pending changes asynchronously
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task<bool> SaveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts a new transaction asynchronously
    /// </summary>
    /// <param name="transactionName">Optional name for the transaction</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Transaction information for the started transaction</returns>
    Task<TransactionInformation> StartTransactionAsync(
        string? transactionName = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Commits changes at a specific transaction point
    /// </summary>
    /// <param name="transactionInformation">The transaction information</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <exception cref="ArgumentNullException">Thrown when transactionInformation is null</exception>
    Task CommitAtTransactionAsync(
        TransactionInformation transactionInformation,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Rolls back changes to a specific transaction point
    /// </summary>
    /// <param name="transactionInformation">The transaction information</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <exception cref="ArgumentNullException">Thrown when transactionInformation is null</exception>
    Task RollBackAtTransactionAsync(
        TransactionInformation transactionInformation,
        CancellationToken cancellationToken = default
    );
}
