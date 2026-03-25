using Microsoft.EntityFrameworkCore.Storage;

namespace TodoApi.Data;

/// <summary>
/// Defines a unit of work pattern for managing database transactions.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Saves all changes made in this unit of work to the database asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing the number of state entries
    /// written to the database.
    /// </returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a new database transaction asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing the database transaction.
    /// </returns>
    Task<IDbContextTransaction> BeginTransactionAsync(
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Commits the specified database transaction asynchronously.
    /// </summary>
    /// <param name="transaction">The database transaction to commit.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CommitTransactionAsync(
        IDbContextTransaction transaction,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Rolls back the specified database transaction asynchronously.
    /// </summary>
    /// <param name="transaction">The database transaction to roll back.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RollbackTransactionAsync(
        IDbContextTransaction transaction,
        CancellationToken cancellationToken = default
    );
}
