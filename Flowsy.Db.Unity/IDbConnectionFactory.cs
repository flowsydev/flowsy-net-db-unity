using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Flowsy.Db.Unity;

/// <summary>
/// Represents a service that creates database connections.
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Gets the default connection key used by the factory.
    /// </summary>
    string DefaultConnectionKey { get; }
    
    /// <summary>
    /// Checks if a configuration exists for a specific connection key.
    /// If no key is provided, the default connection key is used.
    /// </summary>
    /// <param name="connectionKey">
    /// The connection key to check for configuration.
    /// </param>
    /// <returns>
    /// True if a configuration exists for the specified key; otherwise, false.
    /// </returns>
    bool HasConfiguration(string? connectionKey = null);

    /// <summary>
    /// Tries to get the connection configuration for a specific connection key.
    /// If no key is provided, the default connection key is used.
    /// </summary>
    /// <param name="connectionKey">
    /// The connection key for which the configuration is desired.
    /// </param>
    /// <param name="configuration">
    /// When this method returns, contains the connection configuration associated with the specified key,
    /// if the key is found; otherwise, null. This parameter is passed uninitialized.
    /// </param>
    /// <returns>
    /// True if the connection configuration was found; otherwise, false.
    /// </returns>
    bool TryGetConfiguration(string? connectionKey, [MaybeNullWhen(false)] out DbConnectionConfiguration configuration);
    
    /// <summary>
    /// Gets the connection configuration for a specific connection key.
    /// If no key is provided, the default connection key is used.
    /// </summary>
    /// <param name="connectionKey">
    /// The connection key for which the configuration is desired.
    /// </param>
    /// <returns>
    /// An instance of <see cref="DbConnectionConfiguration"/> that contains the connection configuration.
    /// </returns>
    public DbConnectionConfiguration GetConfiguration(string? connectionKey = null);
    
    /// <summary>
    /// Tries to get a database connection using the default connection key.
    /// </summary>
    /// <param name="connection">
    /// When this method returns, contains a database connection associated with the default connection key,
    /// if the connection was successfully created; otherwise, null.
    /// </param>
    /// <returns>
    /// True if the connection was successfully created; otherwise, false.
    /// </returns>
    bool TryGetConnection([MaybeNullWhen(false)] out IDbConnection connection);
    
    /// <summary>
    /// Tries to get a database connection using the default connection key.
    /// </summary>
    /// <param name="open">
    /// Indicates whether the connection should be opened immediately after being created.
    /// </param>
    /// <param name="connection">
    /// When this method returns, contains a database connection associated with the default connection key,
    /// if the connection was successfully created; otherwise, null.
    /// </param>
    /// <returns>
    /// True if the connection was successfully created; otherwise, false.
    /// </returns>
    bool TryGetConnection(bool open, [MaybeNullWhen(false)] out IDbConnection connection);
    
    /// <summary>
    /// Tries to get a database connection using the specified connection key.
    /// </summary>
    /// <param name="connectionKey">
    /// The connection key for which the connection is desired.
    /// </param>
    /// <param name="connection">
    /// When this method returns, contains a database connection associated with the specified connection key,
    /// if the connection was successfully created; otherwise, null.
    /// </param>
    /// <returns>
    /// True if the connection was successfully created; otherwise, false.
    /// </returns>
    bool TryGetConnection(string connectionKey, [MaybeNullWhen(false)] out IDbConnection connection);
    
    /// <summary>
    /// Tries to get a database connection using the specified connection key.
    /// </summary>
    /// <param name="connectionKey">
    /// The connection key for which the connection is desired.
    /// </param>
    /// <param name="open">
    /// Indicates whether the connection should be opened immediately after being created.
    /// </param>
    /// <param name="connection">
    /// When this method returns, contains a database connection associated with the specified connection key,
    /// if the connection was successfully created; otherwise, null.
    /// </param>
    /// <returns>
    /// True if the connection was successfully created; otherwise, false.
    /// </returns>
    bool TryGetConnection(string connectionKey, bool open, [MaybeNullWhen(false)] out IDbConnection connection);

    /// <summary>
    /// Gets a database connection using the specified connection key.
    /// If no key is provided, the default connection key is used.
    /// </summary>
    /// <param name="connectionKey">
    /// The connection key for which the connection is desired.
    /// </param>
    /// <param name="open">
    /// Indicates whether the connection should be opened immediately after being created.
    /// </param>
    /// <returns>
    /// An instance of <see cref="IDbConnection"/> that represents the database connection.
    /// </returns>
    public IDbConnection GetConnection(string? connectionKey = null, bool open = false);
}