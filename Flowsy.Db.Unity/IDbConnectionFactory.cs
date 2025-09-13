using System.Data;

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