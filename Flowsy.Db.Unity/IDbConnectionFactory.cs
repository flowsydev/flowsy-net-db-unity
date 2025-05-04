using System.Data;

namespace Flowsy.Db.Unity;

/// <summary>
/// Obtains database connections based on the provided configuration.
/// Consumers of this service must dispose of the connections when no longer needed.
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Obtains a database connection using the DbConnectionOptions identified by the provided connection key.
    /// </summary>
    /// <param name="connectionKey">
    /// The key that identifies the configuration to use to create the connection.
    /// </param>
    /// <param name="open">
    /// A value indicating whether the connection should be opened.
    /// </param>
    /// <returns>A database connection</returns>
    IDbConnection GetConnection(string connectionKey, bool open = false);
}