using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Flowsy.Db.Unity;

/// <summary>
/// Represents a service that manages database connections without having to worry about opening and closing them.
/// </summary>
public interface IDbConnectionHub : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Gets the default connection key used by the hub.
    /// </summary>
    string DefaultConnectionKey { get; }
    
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
    bool HasConfiguration(string? connectionKey = null);
    
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
    DbConnectionConfiguration GetConfiguration(string? connectionKey = null);
    
    /// <summary>
    /// Gets a database connection associated with the default key.
    /// </summary>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of obtaining the database connection.
    /// </returns>
    Task<IDbConnection> GetConnectionAsync(
        CancellationToken cancellationToken = default
        );
    
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
    Task<IDbConnection> GetConnectionAsync(
        DbConnectionUsage usage,
        CancellationToken cancellationToken = default
        );
    
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
    Task<IDbConnection> GetConnectionAsync(
        DbConnectionUsage usage,
        bool open,
        CancellationToken cancellationToken = default
        );
    
    /// <summary>
    /// Gets a database connection associated with the specified key.
    /// </summary>
    /// <param name="connectionKey">
    /// Connection key to identify the specific connection.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of obtaining the database connection.
    /// </returns>
    Task<IDbConnection> GetConnectionAsync(
        string connectionKey,
        CancellationToken cancellationToken = default
        );
    
    /// <summary>
    /// Gets a database connection associated with the specified key.
    /// </summary>
    /// <param name="connectionKey">
    /// Connection key to identify the specific connection.
    /// </param>
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
    Task<IDbConnection> GetConnectionAsync(
        string connectionKey,
        DbConnectionUsage usage,
        CancellationToken cancellationToken = default
        );
    
    /// <summary>
    /// Gets a database connection associated with the specified key.
    /// </summary>
    /// <param name="connectionKey">
    /// Connection key to identify the specific connection.
    /// </param>
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
    Task<IDbConnection> GetConnectionAsync(
        string connectionKey,
        DbConnectionUsage usage,
        bool open,
        CancellationToken cancellationToken = default
        );

    /// <summary>
    /// Creates a database session using the default connection key.
    /// </summary>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of creating a database session.
    /// </returns>
    Task<IDbSession> CreateSessionAsync(
        CancellationToken cancellationToken = default
        );

    /// <summary>
    /// Creates a database session using the default connection key.
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
    /// A task that represents the asynchronous operation of creating a database session.
    /// </returns>
    Task<IDbSession> CreateSessionAsync(
        DbConnectionUsage usage,
        CancellationToken cancellationToken = default
        );

    /// <summary>
    /// Creates a database session using the default connection key.
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
    /// A task that represents the asynchronous operation of creating a database session.
    /// </returns>
    Task<IDbSession> CreateSessionAsync(
        DbConnectionUsage usage,
        bool open,
        CancellationToken cancellationToken = default
        );

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
    Task<IDbSession> CreateSessionAsync(
        string connectionKey,
        CancellationToken cancellationToken = default
        );

    /// <summary>
    /// Creates a database session using the specified connection key.
    /// </summary>
    /// <param name="connectionKey">
    /// Connection key to identify the connection configuration to use.
    /// </param>
    /// <param name="usage">
    /// Indicates the connection usage, either shared or exclusive.
    /// If the connection is shared, the IDbConnectionHub service handles closing it when disposed (calling Dispose or DisposeAsync).
    /// If the connection is exclusive, the connection consumer is responsible for closing it when no longer needed.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of creating a database session.
    /// </returns>
    Task<IDbSession> CreateSessionAsync(
        string connectionKey,
        DbConnectionUsage usage,
        CancellationToken cancellationToken = default
        );

    /// <summary>
    /// Creates a database session using the specified connection key.
    /// </summary>
    /// <param name="connectionKey">
    /// Connection key to identify the connection configuration to use.
    /// </param>
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
    /// A task that represents the asynchronous operation of creating a database session.
    /// </returns>
    Task<IDbSession> CreateSessionAsync(
        string connectionKey,
        DbConnectionUsage usage,
        bool open,
        CancellationToken cancellationToken = default
        );
    
    /// <summary>
    /// Gets the statistics of the database connection hub.
    /// </summary>
    /// <returns>
    /// An object containing the connection hub statistics, including information about the number of active connections, sessions, and other relevant details.
    /// </returns>
    DbConnectionHubStats GetStats();

    /// <summary>
    /// Clears all connections managed by the hub, closing and releasing associated resources.
    /// </summary>
    void Clear();

    /// <summary>
    /// Clears all connections managed by the hub asynchronously, closing and releasing associated resources.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation of clearing the connections.
    /// </returns>
    Task ClearAsync();

}