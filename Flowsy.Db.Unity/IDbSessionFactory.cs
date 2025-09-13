using System.Data;

namespace Flowsy.Db.Unity;

/// <summary>
/// Represents a factory for creating database sessions.
/// </summary>
public interface IDbSessionFactory
{
    /// <summary>
    /// Creates a database session using the provided connection.
    /// </summary>
    /// <param name="connection">
    /// Database connection that will be used for the session.
    /// </param>
    /// <param name="usage">
    /// Indicates the connection usage, either shared or exclusive.
    /// </param>
    /// <param name="configuration">
    /// The configuration used to create the database connection.
    /// </param>
    /// <returns>
    /// An instance of <see cref="IDbSession"/> that represents the database session created with the provided connection.
    /// </returns>
    IDbSession CreateSession(IDbConnection connection, DbConnectionUsage usage, DbConnectionConfiguration configuration);
}