using System.Data;
using System.Data.Common;
using Dapper;
using Flowsy.Db.Unity.Conventions;
using Flowsy.Db.Unity.Resources;
using Microsoft.Extensions.Logging;

namespace Flowsy.Db.Unity;

/// <summary>
/// Represents a database session that allows performing query operations and transactions.
/// </summary>
public partial class DbSession : IDbSession
{
    private readonly IDbConnection _connection;
    private IDbTransaction? _transaction;
    private readonly ILogger<DbSession>? _logger;
    private bool _disposed;

    /// <summary>
    /// Creates a new instance of the DbSession class.
    /// </summary>
    /// <param name="connection">
    /// The database connection to use for the session.
    /// </param>
    /// <param name="connectionUsage">
    /// Indicates the connection usage, either shared or exclusive.
    /// </param>
    /// <param name="configuration">
    /// The configuration used to create the database connection.
    /// </param>
    /// <param name="logger">
    /// Optional logger for logging operations.
    /// </param>
    public DbSession(IDbConnection connection, DbConnectionUsage connectionUsage, DbConnectionConfiguration configuration, ILogger<DbSession>? logger = null)
    {
        _connection = connection;
        ConnectionUsage = connectionUsage;
        Configuration = configuration;
        _logger = logger;
        SessionId = $"{Configuration.ConnectionKey}/{Ulid.NewUlid()}";
    }

    ~DbSession()
    {
        Dispose(false);
    }

    /// <summary>
    /// Unique key that identifies the database connection.
    /// </summary>
    public string ConnectionKey => Configuration.ConnectionKey;
    
    /// <summary>
    /// Configuration of the database connection.
    /// </summary>
    public DbConnectionConfiguration Configuration { get; }
    
    /// <summary>
    /// Indicates the usage of the database connection.
    /// </summary>
    public DbConnectionUsage ConnectionUsage { get; }

    /// <summary>
    /// Unique identifier of the database session, generated when creating the session.
    /// </summary>
    public string SessionId { get; }
    
    /// <summary>
    /// Generates a new unique identifier for an operation within the session.
    /// </summary>
    /// <returns>A unique string identifier for tracking database operations.</returns>
    protected string CreateOperationId() => Ulid.NewUlid().ToString();

    /// <summary>
    /// Ensures that the database connection is open. If the connection is closed, it opens it.
    /// </summary>
    protected void EnsureOpenConnection()
    {
        if (_connection.State != ConnectionState.Closed)
            return;
        
        var operationId = CreateOperationId();

        _logger?.Log(Configuration.LogLevel, "[ SESSION:{SessionId} > OP:{OperationId} ] Opening connection", SessionId, operationId);

        try
        {
            _connection.Open();

            _logger?.Log(Configuration.LogLevel, "[ SESSION:{SessionId} > OP:{OperationId} ] Connection opened", SessionId, operationId);
        }
        catch (Exception exception)
        {
            _logger?.LogError(exception, "[ SESSION:{SessionId} > OP:{OperationId} ] Error opening connection", SessionId, operationId);
            throw;
        }
    }
    
    /// <summary>
    /// Ensures that the database connection is open. If the connection is closed, it opens it.
    /// </summary>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    protected async Task EnsureOpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        if (_connection.State != ConnectionState.Closed)
            return;
        
        var operationId = CreateOperationId();

        _logger?.Log(Configuration.LogLevel, "[ SESSION:{SessionId} > OP:{OperationId} ] Opening connection", SessionId, operationId);

        try
        {
            if (_connection is DbConnection dbConnection)
                await dbConnection.OpenAsync(cancellationToken);
            else
                _connection.Open();

            _logger?.Log(Configuration.LogLevel, "[ SESSION:{SessionId} > OP:{OperationId} ] Connection opened", SessionId, operationId);
        }
        catch (Exception exception)
        {
            _logger?.LogError(exception, "[ SESSION:{SessionId} > OP:{OperationId} ] Error opening connection", SessionId, operationId);
            throw;
        }
    }

    /// <summary>
    /// Builds a command definition for Dapper with the provided parameters and according to the conventions of this session.
    /// </summary>
    /// <param name="commandText">
    /// Text of the SQL command to execute.
    /// </param>
    /// <param name="parameters">
    /// Optional parameters for the SQL command.
    /// </param>
    /// <param name="commandType">
    /// Type of SQL command (default is CommandType.Text).
    /// </param>
    /// <param name="convention">
    /// Command convention to use (if null, session conventions are used).
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// An instance of <see cref="CommandDefinition"/> configured with the provided parameters and conventions.
    /// </returns>
    protected CommandDefinition BuildCommandDefinition(
        string commandText,
        dynamic? parameters = null,
        CommandType? commandType = null,
        DbCommandConvention? convention = null,
        CancellationToken cancellationToken = default
        )
    {
        var customTimeout = convention?.Timeout;
        var customFlags = convention?.Flags;

        var (defaultTimeout, defaultFlags) = Configuration.Conventions.Commands;

        if (parameters is not SqlMapper.IDynamicParameters dynamicParameters)
        {
            var parameterBuilder = new DbParameterBuilder(Configuration.Conventions, parameters);
            dynamicParameters = parameterBuilder.BuildDynamicParameters();
        }
        
        return new CommandDefinition(
            commandText, 
            dynamicParameters,
            _transaction, 
            customTimeout ?? defaultTimeout, 
            commandType,
            customFlags ?? defaultFlags,
            cancellationToken
            );
    }

    /// <summary>
    /// Builds a command definition for Dapper that calls a routine (stored procedure or function) with the provided parameters and according to the conventions of this session.
    /// </summary>
    /// <param name="routineName">
    /// Name of the routine to call (stored procedure or function).
    /// </param>
    /// <param name="routineType">
    /// Type of routine (if null, session conventions are used).
    /// </param>
    /// <param name="parameters">
    /// Optional parameters for the routine.
    /// </param>
    /// <param name="returnsTable">
    /// Indicates whether the routine returns a table (true) or a scalar value (false).
    /// </param>
    /// <param name="routineConvention">
    /// Routine convention to use (if null, session conventions are used).
    /// </param>
    /// <param name="commandConvention">
    /// Command convention to use (if null, session conventions are used).
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// An instance of <see cref="CommandDefinition"/> configured with the provided parameters and conventions.
    /// </returns>
    protected CommandDefinition BuildCommandDefinition(
        string routineName,
        DbRoutineType? routineType = null,
        object? parameters = null,
        bool returnsTable = false,
        DbRoutineConvention? routineConvention = null,
        DbCommandConvention? commandConvention = null,
        CancellationToken cancellationToken = default
    )
    {
        var conventions = Configuration.Conventions;
        var finalRoutineConvention = routineConvention ?? conventions.Routines;
        
        var routineCall = finalRoutineConvention.PrepareCall(routineName, routineType, parameters, returnsTable);
        
        return BuildCommandDefinition(
            routineCall.Statement,
            routineCall.ParameterBuilder.BuildDynamicParameters(),
            CommandType.Text,
            commandConvention,
            cancellationToken
            );
    }

    /// <summary>
    /// Executes an SQL command and returns the number of affected rows.
    /// </summary>
    /// <param name="command">
    /// SQL command to execute.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// The number of affected rows.
    /// </returns>
    protected async Task<int> ExecuteCommandAsync(IDbCommand command, CancellationToken cancellationToken = default)
    {
        var operationId = CreateOperationId();
        
        _logger?.Log(
            Configuration.LogLevel,
            "[ SESSION:{SessionId} > OP:{OperationId} ] Executing command of type {CommandType}{NewLine}{CommandText}",
            SessionId,
            operationId,
            command.CommandType,
            Environment.NewLine,
            command.CommandText
        );

        try
        {
            int rowsAffected;
            if (command is DbCommand dbCommand)
                rowsAffected = await dbCommand.ExecuteNonQueryAsync(cancellationToken);
            else
                rowsAffected = command.ExecuteNonQuery();

            if (_logger is null)
                return rowsAffected;

            _logger?.Log(
                Configuration.LogLevel,
                "[ SESSION:{SessionId} > OP:{OperationId} ] Command of type {CommandType} executed",
                SessionId,
                operationId,
                command.CommandType
            );

            return rowsAffected;
        }
        catch (Exception exception)
        {
            _logger?.LogError(
                exception,
                "[ SESSION:{SessionId} > OP:{OperationId} ] Error executing command of type {CommandType}",
                SessionId,
                operationId,
                command.CommandType
            );
            throw;
        }
    }
    
    /// <summary>
    /// Executes an SQL command and returns the number of affected rows.
    /// </summary>
    /// <param name="commandText">
    /// SQL command to execute.
    /// </param>
    /// <param name="commandType">
    /// Type of SQL command (default is CommandType.Text).
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// Number of rows affected by the execution of the SQL command.
    /// </returns>
    protected async Task<int> ExecuteCommandAsync(string commandText, CommandType commandType = CommandType.Text, CancellationToken cancellationToken = default)
    {
        await EnsureOpenConnectionAsync(cancellationToken);
        
        using var command = _connection.CreateCommand();
        
        command.CommandText = commandText;
        command.CommandType = commandType;

        var commandConvention = Configuration.Conventions.Commands;
        if (commandConvention.Timeout.HasValue)
            command.CommandTimeout = commandConvention.Timeout.Value; 
        
        command.Transaction = _transaction;

        return await ExecuteCommandAsync(command, cancellationToken);
    }

    /// <summary>
    /// Indicates whether the database session is participating in a transaction.
    /// </summary>
    public bool InTransaction => _transaction is not null;
    
    /// <summary>
    /// Ensures that the session is currently participating in a transaction.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the session is not participating in a transaction.
    /// </exception>
    protected void EnsureInTransaction()
    {
        if (!InTransaction)
            throw new InvalidOperationException(string.Format(Strings.SessionForConnectionXNotParticipatingInTransaction, _connection.ConnectionString));
    }
    
    /// <summary>
    /// Ensures that the session is not currently participating in a transaction.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the session is already participating in a transaction.
    /// </exception>
    protected void EnsureNotInTransaction()
    {
        if (InTransaction)
            throw new InvalidOperationException(string.Format(Strings.SessionForConnectionXIsAlreadyParticipatingInTransaction, _connection.ConnectionString));
    }

    /// <summary>
    /// Starts a transaction in the database session with the default isolation level.
    /// </summary>
    public void BeginTransaction()
        => BeginTransaction(IsolationLevel.ReadCommitted);
    
    /// <summary>
    /// Starts a transaction in the database session with the specified isolation level.
    /// </summary>
    /// <param name="isolationLevel">
    /// Transaction isolation level.
    /// </param>
    public void BeginTransaction(IsolationLevel isolationLevel)
    {
        EnsureNotInTransaction();
        EnsureOpenConnection();
        
        var operationId = CreateOperationId();
        
        _logger?.Log(Configuration.LogLevel, "[ SESSION:{SessionId} > OP:{OperationId} ] Beginning transaction ({IsolationLevel})", SessionId, operationId, isolationLevel);

        try
        {
            _transaction = _connection.BeginTransaction(isolationLevel);
        
            _logger?.Log(Configuration.LogLevel, "[ SESSION:{SessionId} > OP:{OperationId} ] Transaction begun ({IsolationLevel})", SessionId, operationId, isolationLevel);
        }
        catch (Exception exception)
        {
            _logger?.LogError(exception, "[ SESSION:{SessionId} > OP:{OperationId} ] Error beginning transaction ({IsolationLevel})", SessionId, operationId, isolationLevel);
            throw;
        }
    }
    
    /// <summary>
    /// Starts a transaction in the database session asynchronously with the default isolation level.
    /// </summary>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of starting a transaction in the database session.
    /// </returns>
    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        => BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
    
    /// <summary>
    /// Starts a transaction in the database session asynchronously with the specified isolation level.
    /// </summary>
    /// <param name="isolationLevel">
    /// Transaction isolation level.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of starting a transaction in the database session.
    /// </returns>
    public async Task BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default)
    {
        EnsureNotInTransaction();
        await EnsureOpenConnectionAsync(cancellationToken);
        
        var operationId = CreateOperationId();
        
        _logger?.Log(Configuration.LogLevel, "[ SESSION:{SessionId} > OP:{OperationId} ] Beginning transaction ({IsolationLevel})", SessionId, operationId, isolationLevel);

        try
        {
            _transaction = _connection is DbConnection dbConnection 
                ? await dbConnection.BeginTransactionAsync(cancellationToken) 
                : _connection.BeginTransaction(isolationLevel);
        
            _logger?.Log(Configuration.LogLevel, "[ SESSION:{SessionId} > OP:{OperationId} ] Transaction begun ({IsolationLevel})", SessionId, operationId, isolationLevel);
        }
        catch (Exception exception)
        {
            _logger?.LogError(exception, "[ SESSION:{SessionId} > OP:{OperationId} ] Error beginning transaction ({IsolationLevel})", SessionId, operationId, isolationLevel);
            throw;
        }
    }

    /// <summary>
    /// Completes the current transaction in the database session.
    /// </summary>
    public void CommitTransaction()
    {
        EnsureInTransaction();
        
        var isolationLevel = _transaction!.IsolationLevel;
        
        var operationId = CreateOperationId();
        
        _logger?.Log(Configuration.LogLevel, "[ SESSION:{SessionId} > OP:{OperationId} ] Committing transaction ({IsolationLevel})", SessionId, operationId, isolationLevel);

        try
        {
            _transaction.Commit();
            _transaction.Dispose();
            _transaction = null;
        
            _logger?.Log(Configuration.LogLevel, "[ SESSION:{SessionId} > OP:{OperationId} ] Transaction committed ({IsolationLevel})", SessionId, operationId, isolationLevel);
        }
        catch (Exception exception)
        {
            _logger?.LogError(exception, "[ SESSION:{SessionId} > OP:{OperationId} ] Error committing transaction ({IsolationLevel})", SessionId, operationId, isolationLevel);
            throw;
        }
    }

    /// <summary>
    /// Completes the current transaction in the database session asynchronously.
    /// </summary>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of completing the current transaction in the database session.
    /// </returns>
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        EnsureInTransaction();
        
        var isolationLevel = _transaction!.IsolationLevel;
        
        var operationId = CreateOperationId();
        
        _logger?.Log(Configuration.LogLevel, "[ SESSION:{SessionId} > OP:{OperationId} ] Committing transaction ({IsolationLevel})", SessionId, operationId, isolationLevel);

        try
        {
            if (_transaction is DbTransaction dbTransaction)
            {
                await dbTransaction.CommitAsync(cancellationToken);
                await dbTransaction.DisposeAsync();
            }
            else
            {
                _transaction.Commit();
                _transaction.Dispose();
            }
        
            _transaction = null;
        }
        catch (Exception exception)
        {
            _logger?.LogError(exception, "[ SESSION:{SessionId} > OP:{OperationId} ] Error committing transaction ({IsolationLevel})", SessionId, operationId, isolationLevel);
            throw;
        }
    }

    /// <summary>
    /// Attempts to rollback the current transaction if one exists, without throwing exceptions if no transaction is active.
    /// </summary>
    protected void TryRollbackTransaction()
    {
        if (_transaction is null)
            return;
        
        RollbackTransaction();
    }

    /// <summary>
    /// Reverts the current transaction in the database session.
    /// </summary>
    public void RollbackTransaction()
    {
        EnsureInTransaction();
        
        var isolationLevel = _transaction!.IsolationLevel;
        
        var operationId = CreateOperationId();

        _logger?.Log(Configuration.LogLevel, "[ SESSION:{SessionId} > OP:{OperationId} ] Rolling back transaction ({IsolationLevel})", SessionId, operationId, isolationLevel);

        try
        {
            _transaction.Rollback();
            _transaction.Dispose();
            _transaction = null;
        
            _logger?.Log(Configuration.LogLevel, "[ SESSION:{SessionId} > OP:{OperationId} ] Transaction rolled back ({IsolationLevel})", SessionId, operationId, isolationLevel);
        }
        catch (Exception exception)
        {
            _logger?.LogError(exception, "[ SESSION:{SessionId} > OP:{OperationId} ] Error rolling back transaction ({IsolationLevel})", SessionId, operationId, isolationLevel);
            throw;
        }
    }

    /// <summary>
    /// Attempts to rollback the current transaction asynchronously if one exists, without throwing exceptions if no transaction is active.
    /// </summary>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of attempting to rollback the current transaction.
    /// </returns>
    protected async Task TryRollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
            return;
        
        await RollbackTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// Reverts the current transaction in the database session asynchronously.
    /// </summary>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of reverting the current transaction in the database session.
    /// </returns>
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        EnsureInTransaction();
        
        var isolationLevel = _transaction!.IsolationLevel;
        
        var operationId = CreateOperationId();
        
        _logger?.Log(Configuration.LogLevel, "[ SESSION:{SessionId} > OP:{OperationId} ] Rolling back transaction ({IsolationLevel})", SessionId, operationId, isolationLevel);

        try
        {
            switch (_transaction)
            {
                case DbTransaction dbTransaction:
                    await dbTransaction.RollbackAsync(cancellationToken);
                    await dbTransaction.DisposeAsync();
                    break;
                default:
                    _transaction.Rollback();
                    _transaction.Dispose();
                    break;
            }

            _transaction = null;
            
            _logger?.Log(Configuration.LogLevel, "[ SESSION:{SessionId} > OP:{OperationId} ] Transaction rolled back ({IsolationLevel})", SessionId, operationId, isolationLevel);
        }
        catch (Exception exception)
        {
            _logger?.LogError(exception, "[ SESSION:{SessionId} > OP:{OperationId} ] Error rolling back transaction ({IsolationLevel})", SessionId, operationId, isolationLevel);
            throw;
        }
    }

    /// <summary>
    /// Releases all resources used by the <see cref="DbSession"/>.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    /// <param name="disposing">
    /// True to release both managed and unmanaged resources; false to release only unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            if (InTransaction)
            {
                _logger?.LogWarning(
                    "[ SESSION:{SessionId} > OP:{OperationId} ] Disposing session while in transaction ({IsolationLevel}), rolling back",
                    SessionId,
                    CreateOperationId(),
                    _transaction!.IsolationLevel
                    );
            }
            
            TryRollbackTransaction();
            DisposeConnectionIfNeeded();
        }
        
        _disposed = true;
    }

    /// <summary>
    /// Asynchronously releases all resources used by the <see cref="DbSession"/>.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous dispose operation.
    /// </returns>
    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources asynchronously.
    /// </summary>
    /// <param name="disposing">
    /// True to release both managed and unmanaged resources; false to release only unmanaged resources.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous dispose operation.
    /// </returns>
    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            if (InTransaction)
            {
                _logger?.LogWarning(
                    "[ SESSION:{SessionId} > OP:{OperationId} ] Disposing session while in transaction ({IsolationLevel}), rolling back",
                    SessionId,
                    CreateOperationId(),
                    _transaction!.IsolationLevel
                );
            }

            await TryRollbackTransactionAsync();
            await DisposeConnectionIfNeededAsync();
        }

        _disposed = true;
    }
    
    /// <summary>
    /// Disposes or closes the database connection if needed based on the connection usage mode.
    /// </summary>
    protected void DisposeConnectionIfNeeded()
    {
        var operationId = CreateOperationId();
        
        if (ConnectionUsage == DbConnectionUsage.Exclusive)
        {
            _logger?.Log(
                Configuration.LogLevel,
                "[ SESSION:{SessionId} > OP:{OperationId} ] Disposing connection",
                SessionId,
                operationId
            );

            try
            {
                _connection.Dispose();
        
                _logger?.Log(
                    Configuration.LogLevel,
                    "[ SESSION:{SessionId} > OP:{OperationId} ] Connection disposed",
                    SessionId,
                    operationId
                );
                return;
            }
            catch (Exception exception)
            {
                _logger?.LogError(
                    exception,
                    "[ SESSION:{SessionId} > OP:{OperationId} ] Error disposing connection",
                    SessionId,
                    operationId
                );
                throw;
            }
        }
        
        _logger?.Log(
            Configuration.LogLevel,
            "[ SESSION:{SessionId} > OP:{OperationId} ] Closing connection",
            SessionId,
            operationId
        );

        try
        {
            _connection.Close();
        
            _logger?.Log(
                Configuration.LogLevel,
                "[ SESSION:{SessionId} > OP:{OperationId} ] Connection closed",
                SessionId,
                operationId
            );
        }
        catch (Exception exception)
        {
            _logger?.LogError(
                exception,
                "[ SESSION:{SessionId} > OP:{OperationId} ] Error closing connection",
                SessionId,
                operationId
            );
            throw;
        }
    }
    
    /// <summary>
    /// Disposes or closes the database connection asynchronously if needed based on the connection usage mode.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation of disposing or closing the connection.
    /// </returns>
    protected async Task DisposeConnectionIfNeededAsync()
    {
        var operationId = CreateOperationId();
        
        if (ConnectionUsage == DbConnectionUsage.Exclusive)
        {
            _logger?.Log(
                Configuration.LogLevel,
                "[ SESSION:{SessionId} > OP:{OperationId} ] Disposing connection",
                SessionId,
                operationId
            );

            try
            {
                if (_connection is DbConnection dbConnection)
                    await dbConnection.DisposeAsync();
                else
                    _connection.Dispose();
        
                _logger?.Log(
                    Configuration.LogLevel,
                    "[ SESSION:{SessionId} > OP:{OperationId} ] Connection disposed",
                    SessionId,
                    operationId
                );
                return;
            }
            catch (Exception exception)
            {
                _logger?.LogError(
                    exception,
                    "[ SESSION:{SessionId} > OP:{OperationId} ] Error disposing connection",
                    SessionId,
                    operationId
                );
                throw;
            }
        }
        
        _logger?.Log(
            Configuration.LogLevel,
            "[ SESSION:{SessionId} > OP:{OperationId} ] Closing connection",
            SessionId,
            operationId
        );

        try
        {
            if (_connection is DbConnection dbConnection)
                await dbConnection.CloseAsync();
            else
                _connection.Close();
        
            _logger?.Log(
                Configuration.LogLevel,
                "[ SESSION:{SessionId} > OP:{OperationId} ] Connection closed",
                SessionId,
                operationId
            );
        }
        catch (Exception exception)
        {
            _logger?.LogError(
                exception,
                "[ SESSION:{SessionId} > OP:{OperationId} ] Error closing connection",
                SessionId,
                operationId
            );
            throw;
        }
    }
}