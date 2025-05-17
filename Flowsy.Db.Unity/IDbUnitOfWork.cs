using System.Data;

namespace Flowsy.Db.Unity;

/// <summary>
/// Represents a mechanism for executing database operations within a transactional scope.
/// </summary>
public interface IDbUnitOfWork : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// The database connection associated with this unit of work.
    /// </summary>
    IDbConnection Connection { get; }
    
    /// <summary>
    /// The underlying database transaction, if any.
    /// </summary>
    IDbTransaction? Transaction { get; }
    
    /// <summary>
    /// Event raised when the unit of work begins.
    /// </summary>
    public event EventHandler? WorkBegun;
    
    /// <summary>
    /// Event raised when the unit of work is completed successfully.
    /// </summary>
    public event EventHandler? WorkCompleted;
    
    /// <summary>
    /// Event raised when the unit of work is discarded.
    /// </summary>
    public event EventHandler? WorkDiscarded;

    /// <summary>
    /// Begins a new unit of work, establishing a transaction scope.
    /// </summary>
    void BeginWork();
    
    /// <summary>
    /// Indicates whether the unit of work is currently in progress.
    /// </summary>
    bool WorkInProgress { get; }
    
    /// <summary>
    /// Enables the use of another service in the context of this unit of work.
    /// This is useful for sharing the same transaction across multiple services.
    /// </summary>
    /// <param name="participant">
    /// The service to be involved in the unit of work.
    /// </param>
    void Involve(IDbUnitOfWorkParticipant participant);
    
    /// <summary>
    /// Executes the specified action within the context of this unit of work.
    /// </summary>
    /// <param name="action">
    /// The action to be executed within the unit of work.
    /// This action will receive the current database connection and transaction as parameters.
    /// </param>
    void Involve(Action<IDbConnection, IDbTransaction> action);

    /// <summary>
    /// Asynchronously executes the specified action within the context of this unit of work.
    /// </summary>
    /// <param name="action">
    /// The action to be executed within the unit of work.
    /// This action will receive the current database connection and transaction as parameters.
    /// </param>
    /// <param name="cancellationToken">
    /// An optional cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    Task InvolveAsync(Func<IDbConnection, IDbTransaction, CancellationToken, Task> action, CancellationToken cancellationToken = default);

    /// <summary>
    /// Completes the work, committing the underlying transaction.
    /// </summary>
    void CompleteWork();
    
    /// <summary>
    /// Asynchronously completes the work, committing the underlying transaction.
    /// </summary>
    /// <param name="cancellationToken">
    /// An optional cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    Task CompleteWorkAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Discards the work, rolling back the underlying transaction.
    /// </summary>
    void DiscardWork();
    
    /// <summary>
    /// Asynchronously discards the work, rolling back the underlying transaction.
    /// </summary>
    /// <param name="cancellationToken">
    /// An optional cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    Task DiscardWorkAsync(CancellationToken cancellationToken = default);
}