using System.Data;
using System.Data.Common;
using Flowsy.Db.Unity.Extensions;
using Flowsy.Db.Unity.Resources;
using Microsoft.Extensions.Logging;

namespace Flowsy.Db.Unity;

/// <summary>
/// Represents a mechanism for executing database operations within a transactional scope.
/// </summary>
public class DbUnitOfWork : IDbUnitOfWork
{
    private readonly IDbConnectionScope? _connectionScope;
    private IDbConnection? _connection;
    private readonly ILogger? _logger;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="DbUnitOfWork"/> class with the specified connection options.
    /// </summary>
    /// <param name="connectionOptions">
    /// The connection options to be used for the unit of work.
    /// </param>
    /// <param name="logger">
    /// The logger to be used for logging events related to the unit of work.
    /// </param>
    public DbUnitOfWork(DbConnectionOptions connectionOptions, ILogger? logger = null)
    {
        ConnectionOptions = connectionOptions;
        _logger = logger;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DbUnitOfWork"/> class with the specified connection options and service provider.
    /// </summary>
    /// <param name="connectionOptions">
    /// The connection options to be used for the unit of work.
    /// </param>
    /// <param name="connectionScope">
    /// The connection scope to be used for resolving the database connection.
    /// </param>
    /// <param name="logger">
    /// The logger to be used for logging events related to the unit of work.
    /// </param>
    public DbUnitOfWork(DbConnectionOptions connectionOptions, IDbConnectionScope connectionScope, ILogger? logger = null)
    {
        ConnectionOptions = connectionOptions;
        _connectionScope = connectionScope;
        _logger = logger;
    }
    
    ~DbUnitOfWork()
    {
        Dispose(false);
    }

    /// <summary>
    /// Releases the resources used by the <see cref="DbUnitOfWork"/> instance.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    /// <summary>
    /// Releases the resources used by the <see cref="DbUnitOfWork"/> instance.
    /// </summary>
    /// <param name="disposing">
    /// true if the method is called directly or indirectly by user code; false if the method is called by the runtime.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            TryRollbackTransaction();
            _connection?.Dispose();
        }

        _disposed = true;
    }

    /// <summary>
    /// Asynchronously releases the resources used by the <see cref="DbUnitOfWork"/> instance.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Asynchronously releases the resources used by the <see cref="DbUnitOfWork"/> instance.
    /// </summary>
    /// <param name="disposing">
    /// true if the method is called directly or indirectly by user code; false if the method is called by the runtime.
    /// </param>
    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            await TryRollbackTransactionAsync(CancellationToken.None);
            if (_connection is DbConnection dbConnection)
                await dbConnection.DisposeAsync();
            else
                _connection?.Dispose();
        }
        
        _disposed = true;
    }

    /// <summary>
    /// The connection options associated with this unit of work.
    /// </summary>
    protected internal DbConnectionOptions ConnectionOptions { get; }
    
    /// <summary>
    /// The database connection associated with this unit of work.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the connection cannot be resolved for the specified connection key.
    /// </exception>
    public IDbConnection Connection => _connection ??=
        _connectionScope?.GetConnection(ConnectionOptions.ConnectionKey) ??
        throw new InvalidOperationException(string.Format(Strings.CouldNotResolveConnectionForKeyX, ConnectionOptions.ConnectionKey));

    /// <summary>
    /// The underlying database transaction, if any.
    /// </summary>
    public IDbTransaction? Transaction { get; private set; }
    
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
    /// Ensures that the unit of work has begun.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the unit of work has not been started.
    /// </exception>
    protected void EnsureWorkBegun()
    {
        if (Transaction is null)
            throw new InvalidOperationException(string.Format(Strings.MustBeginWorkInvokingMethodX, nameof(BeginWork)));
    }

    /// <summary>
    /// Begins a new unit of work, establishing a transaction scope.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the unit of work has already begun.
    /// </exception>
    public void BeginWork()
    {
        if (Transaction is not null)
            throw new InvalidOperationException(Strings.TheWorkHasAlreadyBegunForThisUnit);
        
        if (Connection.State != ConnectionState.Open)
            Connection.Open();
        
        Transaction = Connection.BeginTransaction();
        OnWorkBegun();
    }

    /// <summary>
    /// Indicates whether the unit of work is currently in progress.
    /// </summary>
    public bool WorkInProgress => Transaction is not null;

    /// <summary>
    /// Triggers the <see cref="WorkBegun"/> event.
    /// </summary>
    protected virtual void OnWorkBegun()
    {
        if (_logger is not null && _logger.IsEnabled(ConnectionOptions.LogLevel))
        {
            _logger.Log(
                ConnectionOptions.LogLevel,
                "[ {DatabaseUrl} ] Unit of work started",
                Connection.GetDatabaseUrl()
            );
        }

        WorkBegun?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Enables the use of another service in the context of this unit of work.
    /// This is useful for sharing the same transaction across multiple services.
    /// </summary>
    /// <param name="participant">
    /// The service to be involved in the unit of work.
    /// </param>
    public void Involve(IDbUnitOfWorkParticipant participant)
    {
        participant.JoinWork(this);
    }

    /// <summary>
    /// Executes the specified action within the context of this unit of work.
    /// </summary>
    /// <param name="action">
    /// The action to be executed within the unit of work.
    /// This action will receive the current database connection and transaction as parameters.
    /// </param>
    public void Involve(Action<IDbConnection, IDbTransaction> action)
    {
        EnsureWorkBegun();
        action(Connection, Transaction!);
    }

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
    public Task InvolveAsync(Func<IDbConnection, IDbTransaction, CancellationToken, Task> action, CancellationToken cancellationToken = default)
    {
        EnsureWorkBegun();
        return action(Connection, Transaction!, cancellationToken);
    }
    
    /// <summary>
    /// Completes the work, committing the underlying transaction.
    /// </summary>
    public void CompleteWork()
    {
        EnsureWorkBegun();
        
        Transaction!.Commit();
        Transaction.Dispose();
        Transaction = null;
        
        OnWorkCompleted();
    }

    /// <summary>
    /// Asynchronously completes the work, committing the underlying transaction.
    /// </summary>
    /// <param name="cancellationToken">
    /// An optional cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    public async Task CompleteWorkAsync(CancellationToken cancellationToken = default)
    {
        EnsureWorkBegun();

        if (Transaction is DbTransaction dbTransaction)
        {
            await dbTransaction.CommitAsync(cancellationToken);
            await dbTransaction.DisposeAsync();
        }
        else
        {
            Transaction!.Commit();
            Transaction.Dispose();
        }
        
        Transaction = null;
        
        OnWorkCompleted();
    }
    
    /// <summary>
    /// Triggers the <see cref="WorkCompleted"/> event.
    /// </summary>
    protected virtual void OnWorkCompleted()
    {
        if (_logger is not null && _logger.IsEnabled(ConnectionOptions.LogLevel))
        {
            _logger.Log(
                ConnectionOptions.LogLevel,
                "[ {DatabaseUrl} ] Unit of work completed",
                Connection.GetDatabaseUrl()
            );
        }
        
        WorkCompleted?.Invoke(this, EventArgs.Empty);
    }
    
    /// <summary>
    /// Attempts to roll back the transaction if it exists.
    /// </summary>
    private void TryRollbackTransaction()
    {
        if (Transaction is null)
            return;
        
        if (_logger is not null && _logger.IsEnabled(ConnectionOptions.LogLevel))
        {
            _logger.Log(
                ConnectionOptions.LogLevel,
                "[ {DatabaseUrl} ] Rolling back transaction",
                Connection.GetDatabaseUrl()
            );
        }
        
        Transaction.Rollback();
        Transaction.Dispose();
        Transaction = null;
        
        if (_logger is not null && _logger.IsEnabled(ConnectionOptions.LogLevel))
        {
            _logger.Log(
                ConnectionOptions.LogLevel,
                "[ {DatabaseUrl} ] Transaction rolled back",
                Connection.GetDatabaseUrl()
            );
        }
    }
    
    /// <summary>
    /// Asynchronously attempts to roll back the transaction if it exists.
    /// </summary>
    /// <param name="cancellationToken">
    /// An optional cancellation token for the asynchronous operation.
    /// </param>
    private async Task TryRollbackTransactionAsync(CancellationToken cancellationToken)
    {
        if (Transaction is null)
            return;
        
        if (_logger is not null && _logger.IsEnabled(ConnectionOptions.LogLevel))
        {
            _logger.Log(
                ConnectionOptions.LogLevel,
                "[ {DatabaseUrl} ] Rolling back transaction",
                Connection.GetDatabaseUrl()
            );
        }
        
        switch (Transaction)
        {
            case DbTransaction dbTransaction:
                await dbTransaction.RollbackAsync(cancellationToken);
                await dbTransaction.DisposeAsync();
                break;
            default:
                Transaction.Rollback();
                Transaction.Dispose();
                break;
        }

        Transaction = null;
        
        if (_logger is not null && _logger.IsEnabled(ConnectionOptions.LogLevel))
        {
            _logger.Log(
                ConnectionOptions.LogLevel,
                "[ {DatabaseUrl} ] Transaction rolled back",
                Connection.GetDatabaseUrl()
            );
        }
    }
    
    /// <summary>
    /// Discards the work, rolling back the underlying transaction.
    /// </summary>
    public void DiscardWork()
    {
        TryRollbackTransaction();
        OnWorkDiscarded();
    }

    /// <summary>
    /// Asynchronously discards the work, rolling back the underlying transaction.
    /// </summary>
    /// <param name="cancellationToken">
    /// An optional cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    public async Task DiscardWorkAsync(CancellationToken cancellationToken = default)
    {
        await TryRollbackTransactionAsync(cancellationToken);
        OnWorkDiscarded();
    }
    
    /// <summary>
    /// Triggers the <see cref="WorkDiscarded"/> event.
    /// </summary>
    protected virtual void OnWorkDiscarded()
    {
        if (_logger is not null && _logger.IsEnabled(ConnectionOptions.LogLevel))
        {
            _logger.Log(
                ConnectionOptions.LogLevel,
                "[ {DatabaseUrl} ] Unit of work discarded",
                Connection.GetDatabaseUrl()
            );
        }
        
        WorkDiscarded?.Invoke(this, EventArgs.Empty);
    }
}