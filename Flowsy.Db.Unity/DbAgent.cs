using System.Data;
using Flowsy.Db.Unity.Extensions;
using Flowsy.Db.Unity.Resources;
using Microsoft.Extensions.Logging;

namespace Flowsy.Db.Unity;

public partial class DbAgent : IDbAgent
{
    private readonly IDbConnectionScope? _connectionScope;
    private IDbConnection? _connection;
    private readonly ILogger? _logger;
    private bool _disposed;

    public DbAgent(DbConnectionOptions connectionOptions, ILogger? logger = null)
    {
        ConnectionOptions = connectionOptions;
    }
    
    public DbAgent(DbConnectionOptions connectionOptions, IDbConnectionScope connectionScope, ILogger? logger = null)
    {
        _connectionScope = connectionScope;
        ConnectionOptions = connectionOptions;
        _logger = logger;
    }

    public DbAgent(IDbUnitOfWork unitOfWork, ILogger? logger = null)
    {
        if (unitOfWork is not DbUnitOfWork dbUnitOfWork)
            throw new ArgumentException(string.Format(Strings.InvalidUnitOfWorkTypeX, nameof(DbUnitOfWork)), nameof(unitOfWork));
        
        ConnectionOptions = dbUnitOfWork.ConnectionOptions;
        UnitOfWork = unitOfWork;
        _logger = logger;
    }

    ~DbAgent() => Dispose(false);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        
        if (disposing)
        {
            if (MustDisposeConnection)
            {
                _connection?.Dispose();
                _connection = null;
            }
        }
        
        _disposed = true;
    }

    public ValueTask DisposeAsync() => DisposeAsync(true);

    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (_disposed) return;
        
        if (disposing)
        {
            if (MustDisposeConnection)
            {
                if (_connection is IAsyncDisposable asyncDisposable)
                    await asyncDisposable.DisposeAsync();
                else
                    _connection?.Dispose();
                _connection = null;
            }
        }
        
        _disposed = true;
    }
    
    protected DbConnectionOptions ConnectionOptions { get; }
    
    public IDbConnection Connection => _connection ??= 
        UnitOfWork?.Connection ??
        _connectionScope?.GetConnection(ConnectionOptions.ConnectionString) ??
        ConnectionOptions.CreateConnection() ??
        throw new InvalidOperationException(string.Format(Strings.CouldNotResolveConnectionForKeyX, ConnectionOptions.ConnectionKey));
    
    public IDbUnitOfWork? UnitOfWork { get; }
    
    protected bool MustDisposeConnection => UnitOfWork is null && _connectionScope is null;
    
    public event DbCommandExecutingEventHandler? CommandExecuting; 
    public event DbCommandExecutedEventHandler? CommandExecuted;
    
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
}