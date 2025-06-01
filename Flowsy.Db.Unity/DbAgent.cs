using System.Data;
using Flowsy.Db.Unity.Extensions;
using Flowsy.Db.Unity.Resources;
using Microsoft.Extensions.Logging;

namespace Flowsy.Db.Unity;

/// <summary>
/// Represents a database agent that performs operations on a database.
/// </summary>
public partial class DbAgent : DbUnitOfWorkParticipant, IDbAgent
{
    private readonly IDbConnectionScope? _connectionScope;
    private IDbConnection? _connection;
    private readonly ILogger? _logger;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="DbAgent"/> class with the specified connection options and optional logger.
    /// </summary>
    /// <param name="connectionOptions">
    /// The connection options to use for database operations.
    /// </param>
    /// <param name="logger">
    /// An optional logger for logging database operations.
    /// </param>
    public DbAgent(DbConnectionOptions connectionOptions, ILogger? logger = null)
    {
        ConnectionOptions = connectionOptions;
        _logger = logger;
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="DbAgent"/> class with the specified connection options, connection scope, and optional logger.
    /// </summary>
    /// <param name="connectionOptions">
    /// The connection options to use for database operations.
    /// </param>
    /// <param name="connectionScope">
    /// The connection scope to use for managing database connections.
    /// </param>
    /// <param name="logger">
    /// An optional logger for logging database operations.
    /// </param>
    public DbAgent(DbConnectionOptions connectionOptions, IDbConnectionScope connectionScope, ILogger? logger = null)
    {
        _connectionScope = connectionScope;
        ConnectionOptions = connectionOptions;
        _logger = logger;
    }

    ~DbAgent() => Dispose(false);

    /// <summary>
    /// Releases the resources used by the <see cref="DbAgent"/> instance.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    /// <summary>
    /// Releases the resources used by the <see cref="DbAgent"/> instance.
    /// </summary>
    /// <param name="disposing">
    /// Indicates whether the method was called directly or by the garbage collector.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        
        if (disposing && OwnsConnection)
        {
            _connection?.Dispose();
            _connection = null;
        }
        
        _disposed = true;
    }

    /// <summary>
    /// Releases the resources used by the <see cref="DbAgent"/> instance.
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
    /// Asynchronously releases the resources used by the <see cref="DbAgent"/> instance.
    /// </summary>
    /// <param name="disposing">
    /// Indicates whether the method was called directly or by the garbage collector.
    /// </param>
    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (_disposed) return;
        
        if (disposing && OwnsConnection)
        {
            if (_connection is IAsyncDisposable asyncDisposable)
                await asyncDisposable.DisposeAsync();
            else
                _connection?.Dispose();
            _connection = null;
        }
        
        _disposed = true;
    }
    
    /// <summary>
    /// The connection options to use for database operations.
    /// </summary>
    protected DbConnectionOptions ConnectionOptions { get; }

    /// <summary>
    /// The database connection associated with this agent.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the connection cannot be resolved.
    /// </exception>
    public IDbConnection Connection
    {
        get
        {
            if (_connection is not null)
                return _connection;
            
            if (UnitOfWork is not null)
            {
                _connection = UnitOfWork.Connection;
                return _connection;
            }
            
            if (_connectionScope is not null)
            {
                _connection = _connectionScope.GetConnection(ConnectionOptions.ConnectionKey);
                return _connection;
            }
            
            _connection = ConnectionOptions.CreateConnection();
            return _connection;
        }
    } 

    /// <summary>
    /// Indicates whether this service owns the connection and must dispose of it when destroyed.
    /// </summary>
    protected bool OwnsConnection => !IsParticipating && _connectionScope is null;
    
    /// <summary>
    /// Raised when a command is about to be executed.
    /// </summary>
    public event DbCommandExecutingEventHandler? CommandExecuting;
    
    /// <summary>
    /// Raised when a command has been executed.
    /// </summary>
    public event DbCommandExecutedEventHandler? CommandExecuted;
    
    /// <summary>
    /// Raises the <see cref="CommandExecuting"/> event.
    /// </summary>
    /// <param name="e">
    /// The event arguments containing information about the command being executed.
    /// </param>
    protected virtual void OnCommandExecuting(DbCommandExecutingEventArgs e)
    {
        if (_connection is not null && _logger is not null && _logger.IsEnabled(ConnectionOptions.LogLevel))
        {
            var command = e.CommandDefinition;
            _logger.Log(
                ConnectionOptions.LogLevel,
                "[ {DatabaseUrl} ] Command executing: {CommandText}",
                _connection.GetDatabaseUrl(),
                command.CommandText
            );
        }
        CommandExecuting?.Invoke(this, e);
    }
    
    /// <summary>
    /// Raises the <see cref="CommandExecuted"/> event.
    /// </summary>
    /// <param name="e">
    /// The event arguments containing information about the executed command.
    /// </param>
    protected virtual void OnCommandExecuted(DbCommandExecutedEventArgs e)
    {
        if (_connection is not null && _logger is not null && _logger.IsEnabled(ConnectionOptions.LogLevel))
        {
            var commandText = e.CommandDefinition.CommandText;
            var databaseUrl = _connection.GetDatabaseUrl();
            if (e.Result is not null && ConnectionOptions.LogLevel is LogLevel.Trace or LogLevel.Debug)
            {
                _logger.Log(
                    ConnectionOptions.LogLevel,
                    "[ {DatabaseUrl} ] Command executed: {CommandText}{NewLine}{@Result}",
                    databaseUrl,
                    commandText,
                    Environment.NewLine,
                    e.Result
                    );
            }
            else
            {
                _logger.Log(
                    ConnectionOptions.LogLevel,
                    "[ {DatabaseUrl} ] Command executed: {CommandText}",
                    databaseUrl,
                    commandText
                );
            }
        }
        CommandExecuted?.Invoke(this, e);
    }

    public override void Join(IDbUnitOfWork unitOfWork)
    {
        if (OwnsConnection)
        {
            _connection?.Dispose();
            _connection = null;
        }
        base.Join(unitOfWork);
    }

    public override void Leave()
    {
        base.Leave();
        _connection = null;
    }
}