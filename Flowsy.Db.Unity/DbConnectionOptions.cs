using System.Data;
using Flowsy.Db.Unity.Conventions;
using Flowsy.Db.Unity.Resources;
using Microsoft.Extensions.Logging;

namespace Flowsy.Db.Unity;

/// <summary>
/// Options for database operations.
/// An instance of this class is associated to a specific database connection using the ConnectionKey property.
/// </summary>
public class DbConnectionOptions
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    public DbConnectionOptions()
    {
    }
 
    /// <summary>
    /// Constructor that initializes the connection options with a connection key.
    /// </summary>
    /// <param name="connectionKey">
    /// The unique key for the database connection.
    /// </param>
    public DbConnectionOptions(string connectionKey)
    {
        ConnectionKey = connectionKey;
    }

    /// <summary>
    /// The unique key for the database connection.
    /// </summary>
    public string ConnectionKey { get; private set; } = string.Empty;
    
    /// <summary>
    /// The database provider descriptor.
    /// </summary>
    public DbProviderDescriptor Provider { get; internal set; } = DbProviderDescriptor.Generic;
    
    /// <summary>
    /// The connection string for the database.
    /// </summary>
    public string ConnectionString { get; internal set; } = string.Empty;
    
    /// <summary>
    /// Indicates whether this connection is the default connection.
    /// </summary>
    public bool Default { get; internal set; }
    
    /// <summary>
    /// The type of the connection factory to use for creating database connections.
    /// </summary>
    public Type ConnectionFactoryType { get; internal set; } = typeof(DbConnectionFactory);
    
    public Type AgentType { get; internal set; } = typeof(DbAgent);
    
    public Type UnitOfWorkType { get; internal set; } = typeof(DbUnitOfWork);
    
    public DbConventionSet? Conventions { get; internal set; }
    
    public LogLevel LogLevel { get; internal set; } = LogLevel.Information;

    public IDbConnection CreateConnection()
    {
        var connection = Provider.Factory?.CreateConnection();
        if (connection is null)
            throw new InvalidOperationException(string.Format(Strings.FailedToCreateConnectionUsingProviderX, Provider.InvariantName));
        
        connection.ConnectionString = ConnectionString;
        return connection;
    }

    public void CopyTo(DbConnectionOptions other)
    {
        other.ConnectionKey = ConnectionKey;
        other.Provider = Provider;
        other.ConnectionString = ConnectionString;
        other.Default = Default;
        other.Conventions = Conventions?.Clone();
        other.LogLevel = LogLevel;
        other.ConnectionFactoryType = ConnectionFactoryType;
        other.AgentType = AgentType;
        other.UnitOfWorkType = UnitOfWorkType;
    }
}