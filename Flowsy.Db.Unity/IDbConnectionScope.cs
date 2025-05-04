using System.Data;

namespace Flowsy.Db.Unity;

/// <summary>
/// Represents a wrapper around a collection of database connections.
/// The connections will be automatically disposed when the IDbConnectionScope is disposed.
/// </summary>
public interface IDbConnectionScope: IDisposable, IAsyncDisposable
{
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
    IDbConnection GetConnection(string connectionKey, bool open = false);
}