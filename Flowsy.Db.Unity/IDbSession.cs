using System.Data;

namespace Flowsy.Db.Unity;

/// <summary>
/// Represents a database session that allows performing query operations and transactions.
/// </summary>
public partial interface IDbSession : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Unique key that identifies the database connection.
    /// </summary>
    string ConnectionKey { get; }
    
    /// <summary>
    /// Configuration of the database connection.
    /// </summary>
    DbConnectionConfiguration Configuration { get; }
    
    /// <summary>
    /// Indicates the usage of the database connection.
    /// </summary>
    DbConnectionUsage ConnectionUsage { get; }
    
    /// <summary>
    /// Unique identifier of the database session.
    /// </summary>
    string SessionId { get; }

    /// <summary>
    /// Indicates whether the database session is participating in a transaction.
    /// </summary>
    bool InTransaction { get; }
    
    /// <summary>
    /// Starts a transaction in the database session.
    /// </summary>
    void BeginTransaction();
    
    /// <summary>
    /// Starts a transaction in the database session.
    /// </summary>
    /// <param name="isolationLevel">
    /// Transaction isolation level. Default is <see cref="IsolationLevel.ReadCommitted"/>.
    /// </param>
    void BeginTransaction(IsolationLevel isolationLevel);
    
    /// <summary>
    /// Starts a transaction in the database session asynchronously.
    /// </summary>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of starting a transaction in the database session.
    /// </returns>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Starts a transaction in the database session asynchronously.
    /// </summary>
    /// <param name="isolationLevel">
    /// Transaction isolation level. Default is <see cref="IsolationLevel.ReadCommitted"/>.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of starting a transaction in the database session.
    /// </returns>
    Task BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Completes the current transaction in the database session.
    /// </summary>
    void CommitTransaction();
    
    /// <summary>
    /// Completes the current transaction in the database session asynchronously.
    /// </summary>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of completing the current transaction in the database session.
    /// </returns>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Reverts the current transaction in the database session.
    /// </summary>
    void RollbackTransaction();
    
    /// <summary>
    /// Reverts the current transaction in the database session asynchronously.
    /// </summary>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of reverting the current transaction in the database session.
    /// </returns>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}