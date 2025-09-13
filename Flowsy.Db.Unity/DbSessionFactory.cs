using System.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Flowsy.Db.Unity;

/// <summary>
/// Represents a factory for creating database sessions.
/// </summary>
public class DbSessionFactory : IDbSessionFactory
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Creates a new instance of the DbSessionFactory class.
    /// </summary>
    /// <param name="serviceProvider">
    /// The service provider used to resolve dependencies.
    /// </param>
    public DbSessionFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

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
    public IDbSession CreateSession(IDbConnection connection, DbConnectionUsage usage, DbConnectionConfiguration configuration)
        => new DbSession(connection, usage, configuration, _serviceProvider.GetService<ILogger<DbSession>>());
}