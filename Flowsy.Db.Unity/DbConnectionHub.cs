using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using Microsoft.Extensions.Logging;

namespace Flowsy.Db.Unity;

/// <summary>
/// Represents a service that manages database connections without having to worry about opening and closing them.
/// </summary>
public class DbConnectionHub : IDbConnectionHub
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IDbSessionFactory _sessionFactory;
    private readonly ConcurrentDictionary<string, Lazy<Task<IDbConnection>>> _sharedConnections = [];
    private readonly SemaphoreSlim _sharedConnectionOpeningLock = new(1, 1);
    private readonly ConcurrentQueue<(string Key, IDbConnection Connection)> _exclusiveConnections = [];
    private readonly ILogger<DbConnectionHub>? _logger;
    private bool _disposed;
    
    /// <summary>
    /// Creates a new instance of the DbConnectionHub class.
    /// </summary>
    /// <param name="connectionFactory">
    /// The connection factory used to create database connections.
    /// </param>
    /// <param name="sessionFactory">
    /// The session factory used to create database sessions.
    /// </param>
    /// <param name="logger">
    /// Optional logger for logging operations.
    /// </param>
    public DbConnectionHub(IDbConnectionFactory connectionFactory, IDbSessionFactory sessionFactory, ILogger<DbConnectionHub>? logger = null)
    {
        _connectionFactory = connectionFactory;
        _sessionFactory = sessionFactory;
        _logger = logger;
    }

    ~DbConnectionHub()
    {
        Dispose(false);
    }

    /// <summary>
    /// Gets the default connection key used by the hub.
    /// </summary>
    public string DefaultConnectionKey => _connectionFactory.DefaultConnectionKey;

    /// <summary>
    /// Checks if a database connection configuration exists for the specified key.
    /// If no key is provided, the default connection key is used.
    /// </summary>
    /// <param name="connectionKey">
    /// Connection key to identify the connection configuration to check.
    /// </param>
    /// <returns>
    /// True if the configuration exists; otherwise, false.
    /// </returns>
    public bool HasConfiguration(string? connectionKey = null) => _connectionFactory.HasConfiguration(connectionKey);

    /// <summary>
    /// Gets the database connection configuration associated with the specified key.
    /// If no key is provided, the default connection key is used.
    /// </summary>
    /// <param name="connectionKey">
    /// Connection key to identify the connection configuration to use.
    /// </param>
    /// <returns>
    /// The database connection configuration.
    /// </returns>
    public DbConnectionConfiguration GetConfiguration(string? connectionKey) => _connectionFactory.GetConfiguration(connectionKey);

    /// <summary>
    /// Gets a database connection associated with the default key.
    /// </summary>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of obtaining the database connection.
    /// </returns>
    public Task<IDbConnection> GetConnectionAsync(
        CancellationToken cancellationToken = default
        ) => GetConnectionAsync(DbConnectionUsage.Shared, false, cancellationToken);

    /// <summary>
    /// Gets a database connection associated with the default key.
    /// </summary>
    /// <param name="usage">
    /// Indicates the connection usage, either shared or exclusive.
    /// If the connection is shared, the IDbConnectionHub service handles closing it when disposed (calling Dispose or DisposeAsync).
    /// If the connection is exclusive, the connection consumer is responsible for closing it when no longer needed.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of obtaining the database connection.
    /// </returns>
    public Task<IDbConnection> GetConnectionAsync(
        DbConnectionUsage usage,
        CancellationToken cancellationToken = default
        ) => GetConnectionAsync(usage, false, cancellationToken);
    

    /// <summary>
    /// Gets a database connection associated with the default key.
    /// </summary>
    /// <param name="usage">
    /// Indicates the connection usage, either shared or exclusive.
    /// If the connection is shared, the IDbConnectionHub service handles closing it when disposed (calling Dispose or DisposeAsync).
    /// If the connection is exclusive, the connection consumer is responsible for closing it when no longer needed.
    /// </param>
    /// <param name="open">
    /// Indicates whether the connection should be opened immediately.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of obtaining the database connection.
    /// </returns>
    public Task<IDbConnection> GetConnectionAsync(
        DbConnectionUsage usage,
        bool open,
        CancellationToken cancellationToken = default
        ) => GetConnectionAsync(_connectionFactory.DefaultConnectionKey, usage, open, cancellationToken);

    /// <summary>
    /// Gets a database connection associated with the specified key.
    /// </summary>
    /// <param name="connectionKey">
    /// Connection key to identify the specific connection.
    /// </param>
    /// <param name="usage">
    /// Indicates the connection usage, whether shared or exclusive.
    /// If the connection is shared, the IDbConnectionHub service handles closing it when disposed (by calling Dispose or DisposeAsync).
    /// If the connection is exclusive, the connection consumer is responsible for closing it when no longer needed.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of getting the database connection.
    /// </returns>
    public Task<IDbConnection> GetConnectionAsync(
        string connectionKey,
        CancellationToken cancellationToken = default
        ) => GetConnectionAsync(connectionKey, DbConnectionUsage.Shared, false, cancellationToken);

    /// <summary>
    /// Gets a database connection associated with the specified key.
    /// </summary>
    /// <param name="connectionKey">
    /// Connection key to identify the specific connection.
    /// </param>
    /// <param name="usage">
    /// Indicates the connection usage, whether shared or exclusive.
    /// If the connection is shared, the IDbConnectionHub service handles closing it when disposed (by calling Dispose or DisposeAsync).
    /// If the connection is exclusive, the connection consumer is responsible for closing it when no longer needed.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of getting the database connection.
    /// </returns>
    public Task<IDbConnection> GetConnectionAsync(
        string connectionKey,
        DbConnectionUsage usage,
        CancellationToken cancellationToken = default
        ) => GetConnectionAsync(connectionKey, usage, false, cancellationToken);
    

    /// <summary>
    /// Gets a database connection associated with the specified key.
    /// </summary>
    /// <param name="connectionKey">
    /// Connection key to identify the specific connection.
    /// </param>
    /// <param name="usage">
    /// Indicates the connection usage, whether shared or exclusive.
    /// If the connection is shared, the IDbConnectionHub service handles closing it when disposed (by calling Dispose or DisposeAsync).
    /// If the connection is exclusive, the connection consumer is responsible for closing it when no longer needed.
    /// </param>
    /// <param name="open">
    /// Indicates whether the connection should be opened immediately.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of getting the database connection.
    /// </returns>
    public async Task<IDbConnection> GetConnectionAsync(
        string connectionKey,
        DbConnectionUsage usage,
        bool open,
        CancellationToken cancellationToken = default
        )
    {
        if (_disposed) throw new ObjectDisposedException(nameof(DbConnectionHub));
        
        var key = string.IsNullOrEmpty(connectionKey) ? _connectionFactory.DefaultConnectionKey : connectionKey;
        
        if (usage == DbConnectionUsage.Exclusive)
            return await GetExclusiveConnectionAsync(key, open, cancellationToken);
        
        return await GetSharedConnectionAsync(key, open, cancellationToken);
    }

    /// <summary>
    /// Creates a database session using the default connection key.
    /// </summary>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of creating a database session.
    /// </returns>
    public Task<IDbSession> CreateSessionAsync(
        CancellationToken cancellationToken = default
        ) => CreateSessionAsync(DbConnectionUsage.Shared, false, cancellationToken);

    /// <summary>
    /// Creates a database session using the default connection key.
    /// </summary>
    /// <param name="usage">
    /// Indicates the connection usage, whether shared or exclusive.
    /// If the connection is shared, the IDbConnectionHub service handles closing it when disposed (by calling Dispose or DisposeAsync).
    /// If the connection is exclusive, the connection consumer is responsible for closing it when no longer needed.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of creating a database session.
    /// </returns>
    public Task<IDbSession> CreateSessionAsync(
        DbConnectionUsage usage,
        CancellationToken cancellationToken = default
        ) => CreateSessionAsync(usage, false, cancellationToken);

    /// <summary>
    /// Creates a database session using the default connection key.
    /// </summary>
    /// <param name="usage">
    /// Indicates the connection usage, whether shared or exclusive.
    /// If the connection is shared, the IDbConnectionHub service handles closing it when disposed (by calling Dispose or DisposeAsync).
    /// If the connection is exclusive, the connection consumer is responsible for closing it when no longer needed.
    /// </param>
    /// <param name="open">
    /// Indicates whether the connection should be opened immediately.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of creating a database session.
    /// </returns>
    public Task<IDbSession> CreateSessionAsync(
        DbConnectionUsage usage,
        bool open,
        CancellationToken cancellationToken = default
        ) => CreateSessionAsync(_connectionFactory.DefaultConnectionKey, usage, open, cancellationToken);

    /// <summary>
    /// Creates a database session using the specified connection key.
    /// </summary>
    /// <param name="connectionKey">
    /// Connection key to identify the connection configuration to use.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of creating a database session.
    /// </returns>
    public Task<IDbSession> CreateSessionAsync(
        string connectionKey,
        CancellationToken cancellationToken = default
        ) => CreateSessionAsync(connectionKey, DbConnectionUsage.Shared, false, cancellationToken);

    /// <summary>
    /// Creates a database session using the specified connection key.
    /// </summary>
    /// <param name="connectionKey">
    /// Connection key to identify the connection configuration to use.
    /// </param>
    /// <param name="usage">
    /// Indicates the connection usage, whether shared or exclusive.
    /// If the connection is shared, the IDbConnectionHub service handles closing it when disposed (by calling Dispose or DisposeAsync).
    /// If the connection is exclusive, the connection consumer is responsible for closing it when no longer needed.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of creating a database session.
    /// </returns>
    public Task<IDbSession> CreateSessionAsync(
        string connectionKey,
        DbConnectionUsage usage,
        CancellationToken cancellationToken = default
        ) => CreateSessionAsync(connectionKey, usage, false, cancellationToken);

    /// <summary>
    /// Creates a database session using the specified connection key.
    /// </summary>
    /// <param name="connectionKey">
    /// Connection key to identify the connection configuration to use.
    /// </param>
    /// <param name="usage">
    /// Indicates the connection usage, whether shared or exclusive.
    /// If the connection is shared, the IDbConnectionHub service handles closing it when disposed (by calling Dispose or DisposeAsync).
    /// If the connection is exclusive, the connection consumer is responsible for closing it when no longer needed.
    /// </param>
    /// <param name="open">
    /// Indicates whether the connection should be opened immediately.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of creating a database session.
    /// </returns>
    public async Task<IDbSession> CreateSessionAsync(
        string connectionKey,
        DbConnectionUsage usage,
        bool open,
        CancellationToken cancellationToken = default
        )
    {
        if (_disposed) throw new ObjectDisposedException(nameof(DbConnectionHub));
        
        var key = string.IsNullOrEmpty(connectionKey) ? _connectionFactory.DefaultConnectionKey : connectionKey;
        
        var configuration = _connectionFactory.GetConfiguration(key);
        
        var connection = await GetConnectionAsync(key, usage, open, cancellationToken).ConfigureAwait(false);
        
        return _sessionFactory.CreateSession(connection, usage, configuration);
    }

    /// <summary>
    /// Gets an exclusive database connection.
    /// The connection will not be shared with other consumers and will be automatically closed when the DbConnectionHub is disposed.
    /// </summary>
    /// <param name="connectionKey">
    /// Connection key to identify the specific connection.
    /// </param>
    /// <param name="open">
    /// Indicates whether the connection should be opened immediately.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of getting the exclusive database connection.
    /// </returns>
    private async Task<IDbConnection> GetExclusiveConnectionAsync(string connectionKey, bool open = false, CancellationToken cancellationToken = default)
    {
        var connection = _connectionFactory.GetConnection(connectionKey);
        if (!open) return connection;
        
        await OpenIfNeededAsync(connection, cancellationToken).ConfigureAwait(false);
        
        _exclusiveConnections.Enqueue((connectionKey, connection));
        
        return connection;
    }
    
    /// <summary>
    /// Gets a shared database connection.
    /// The connection can be shared with other consumers and will be automatically closed when the DbConnectionHub is disposed.
    /// </summary>
    /// <param name="connectionKey">
    /// Connection key to identify the specific connection.
    /// </param>
    /// <param name="open">
    /// Indicates whether the connection should be opened immediately.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of getting the shared database connection.
    /// </returns>
    private async Task<IDbConnection> GetSharedConnectionAsync(string connectionKey, bool open = false, CancellationToken cancellationToken = default)
    {
        var lazyTask = _sharedConnections.GetOrAdd(connectionKey, k => new Lazy<Task<IDbConnection>>(async () =>
        {
            var c = _connectionFactory.GetConnection(k);
            if (!open) return c;
            await OpenIfNeededAsync(c, cancellationToken).ConfigureAwait(false);
            return c;
        }));
        
        var connection = await lazyTask.Value.ConfigureAwait(false);
        
        // If no need to open the connection, return it directly.
        if (!open) return connection;
        
        // Open safely if some consumer closed the connection and it is required to be open.
        await _sharedConnectionOpeningLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await OpenIfNeededAsync(connection, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _sharedConnectionOpeningLock.Release(); 
        }
        return connection;
    }
    
    /// <summary>
    /// Opens the connection if it is closed.
    /// </summary>
    /// <param name="connection">
    /// The connection that should be opened if it is closed.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    private static async Task OpenIfNeededAsync(IDbConnection connection, CancellationToken cancellationToken = default)
    {
        if (connection.State != ConnectionState.Closed)
            return;
        
        if (connection is DbConnection dbConnection)
            await dbConnection.OpenAsync(cancellationToken).ConfigureAwait(false);
        else
            connection.Open();
    }
    
    public DbConnectionHubStats GetStats()
    {
        var sharedConnectionGrouping = _sharedConnections
            .Where(kvp => kvp.Value.IsValueCreated)
            .Select(kvp => new
            {
                kvp.Key,
                Connection = kvp.Value.Value.Result
            })
            .GroupBy(x => new
            {
                ConnectionKey = x.Key,
                ConnectionType = x.Connection.GetType(),
                ConnectionState = x.Connection.State
            })
            .ToDictionary(g => g.Key, g => g.Count());
        
        var sharedStats = new Dictionary<string, DbConnectionGroupStats>();
        foreach (var (key, stateCount) in sharedConnectionGrouping)
        {
            if (!sharedStats.TryGetValue(key.ConnectionKey, out var groupStats))
            {
                groupStats = new DbConnectionGroupStats(key.ConnectionKey, key.ConnectionType, 0, 0, 0, 0, 0, 0);
            }

            groupStats = key.ConnectionState switch
            {
                ConnectionState.Closed => groupStats with {ClosedCount = groupStats.ClosedCount + stateCount},
                ConnectionState.Open => groupStats with {OpenCount = groupStats.OpenCount + stateCount},
                ConnectionState.Connecting => groupStats with {ConnectingCount = groupStats.ConnectingCount + stateCount},
                ConnectionState.Executing => groupStats with {ExecutingCount = groupStats.ExecutingCount + stateCount},
                ConnectionState.Fetching => groupStats with {FetchingCount = groupStats.FetchingCount + stateCount},
                ConnectionState.Broken => groupStats with {BrokenCount = groupStats.BrokenCount + stateCount},
                _ => groupStats
            };

            sharedStats[key.ConnectionKey] = groupStats;
        }
        
        var exclusiveConnectionGrouping = _exclusiveConnections
            .GroupBy(e => new
            {
                ConnectionKey = e.Key,
                ConnectionType = e.Connection.GetType(),
                ConnectionState = e.Connection.State
            })
            .ToDictionary(g => g.Key, g => g.Count());

        var exclusiveStats = new Dictionary<string, DbConnectionGroupStats>();
        foreach (var (key, stateCount) in exclusiveConnectionGrouping)
        {
            if (!exclusiveStats.TryGetValue(key.ConnectionKey, out var groupStats))
            {
                groupStats = new DbConnectionGroupStats(key.ConnectionKey, key.ConnectionType, 0, 0, 0, 0, 0, 0);
            }

            groupStats = key.ConnectionState switch
            {
                ConnectionState.Closed => groupStats with {ClosedCount = groupStats.ClosedCount + stateCount},
                ConnectionState.Open => groupStats with {OpenCount = groupStats.OpenCount + stateCount},
                ConnectionState.Connecting => groupStats with {ConnectingCount = groupStats.ConnectingCount + stateCount},
                ConnectionState.Executing => groupStats with {ExecutingCount = groupStats.ExecutingCount + stateCount},
                ConnectionState.Fetching => groupStats with {FetchingCount = groupStats.FetchingCount + stateCount},
                ConnectionState.Broken => groupStats with {BrokenCount = groupStats.BrokenCount + stateCount},
                _ => groupStats
            };

            exclusiveStats[key.ConnectionKey] = groupStats;
        }
        
        return new DbConnectionHubStats(sharedStats, exclusiveStats);
    }

    /// <summary>
    /// Clears all connections managed by the hub, closing and releasing associated resources.
    /// </summary>
    public void Clear()
    {
        // Cerrar y disponer todas las conexiones exclusivas.
        while (_exclusiveConnections.TryDequeue(out var entry))
        {
            try
            {
                entry.Connection.Dispose();
            }
            catch (Exception ex)
            {
                if (_logger != null && _logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning(ex, "Error disposing exclusive connection {Connection}", entry.Connection);
            }
        }
        _exclusiveConnections.Clear();
        
        // Cerrar y disponer todas las conexiones compartidas.
        foreach (var (connectionKey, lazyTask) in _sharedConnections)
        {
            if (!lazyTask.IsValueCreated) continue;
            
            try
            {
                var task = lazyTask.Value;
                task.Wait();
            
                var connection = task.Result;
                connection.Dispose();
            }
            catch(Exception ex)
            {
                if (_logger != null && _logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning(ex, "Error disposing exclusive connection for key {ConnectionKey}", connectionKey);
            }
        }
        _sharedConnections.Clear();
    }
    
    /// <summary>
    /// Clears all connections managed by the hub asynchronously, closing and releasing associated resources.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation of clearing the connections.
    /// </returns>
    public async Task ClearAsync()
    {
        // Cerrar y disponer todas las conexiones exclusivas.
        while (_exclusiveConnections.TryDequeue(out var entry))
        {
            try
            {
                switch (entry.Connection)
                {
                    case IAsyncDisposable ad:
                        await ad.DisposeAsync().ConfigureAwait(false);
                        break;
                    default:
                        entry.Connection.Dispose();
                        break;
                }
            }
            catch (Exception ex)
            {
                if (_logger != null && _logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning(ex, "Error disposing exclusive connection {Connection}", entry.Connection);
            }
        }
        _exclusiveConnections.Clear();
        
        // Cerrar y disponer todas las conexiones compartidas.
        foreach (var (connectionKey, lazyTask) in _sharedConnections)
        {
            if (!lazyTask.IsValueCreated) continue;
            
            try
            {
                var task = lazyTask.Value;
                var connection = await task.ConfigureAwait(false);
                
                switch (connection)
                {
                    case IAsyncDisposable ad:
                        await ad.DisposeAsync().ConfigureAwait(false);
                        break;
                    default:
                        connection.Dispose();
                        break;
                }
            }
            catch(Exception ex)
            {
                if (_logger != null && _logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning(ex, "Error disposing shared connection for key {ConnectionKey}", connectionKey);
            }
        }
        _sharedConnections.Clear();
    }
    
    /// <summary>
    /// Releases the resources used by the DbConnectionHub.
    /// </summary>
    public void Dispose()
    {
        try
        {
            Dispose(true);
        }
        finally
        {
            GC.SuppressFinalize(this);   
        }
    }
    
    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            Clear();
            _sharedConnectionOpeningLock.Dispose();
        }
        
        _disposed = true;
    }

    /// <summary>
    /// Asynchronously releases the resources used by the DbConnectionHub.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        try
        {
            await DisposeAsync(true).ConfigureAwait(false);
        }
        finally
        {
            GC.SuppressFinalize(this);
        }
    }

    private async ValueTask DisposeAsync(bool disposing)
    {
        if (_disposed) return;
        
        if (disposing)
        {
            await ClearAsync();
            _sharedConnectionOpeningLock.Dispose();   
        }
        
        _disposed = true;
    }
}

