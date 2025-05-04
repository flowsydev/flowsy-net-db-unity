using System.Collections.Concurrent;
using System.Data;

namespace Flowsy.Db.Unity;

/// <summary>
/// Represents a wrapper around a collection of database connections.
/// The connections will be automatically disposed when the IDbConnectionScope is disposed.
/// </summary>
public class DbConnectionScope : IDbConnectionScope
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ConcurrentDictionary<string, IDbConnection> _connections = new();
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="DbConnectionScope"/> class.
    /// </summary>
    /// <param name="connectionFactory"></param>
    public DbConnectionScope(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    
    ~DbConnectionScope()
    {
        Dispose(false);
    }
    
    /// <summary>
    /// Gets a database connection.
    /// If the connection already exists in this scope, it will be returned.
    /// When this scope is disposed, all connections will be disposed.
    /// </summary>
    /// <param name="connectionKey">
    /// A value from the keys used to configure the connection factory.
    /// </param>
    /// <param name="open">
    /// Whether to open the connection after creating it.
    /// </param>
    /// <returns>
    /// The database connection.
    /// </returns>
    public IDbConnection GetConnection(string connectionKey, bool open = false)
    {
        if (_connections.TryGetValue(connectionKey, out var connection))
            return connection;

        connection = _connectionFactory.GetConnection(connectionKey, open);
        _connections[connectionKey] = connection;
        return connection;
    }

    /// <summary>
    /// Releases connections from the scope.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;
        
        if (disposing)
        {
            foreach (var connection in _connections.Values)
                connection.Dispose();
            
            _connections.Clear();
        }
        
        _disposed = true;
    }

    /// <summary>
    /// Asynchronously releases connections from the scope.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }
    
    private async ValueTask DisposeAsync(bool disposing)
    {
        if (_disposed)
            return;
        
        if (disposing)
        {
            foreach (var connection in _connections.Values)
            {
                if (connection is IAsyncDisposable asyncDisposable)
                    await asyncDisposable.DisposeAsync();
                else
                    connection.Dispose();
            }
            _connections.Clear();
        }
        
        _disposed = true;
    }
}