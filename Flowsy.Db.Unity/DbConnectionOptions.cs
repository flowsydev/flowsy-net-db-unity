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
    /// The set of conventions to be used for this connection.
    /// </summary>
    public DbConventionSet? Conventions { get; internal set; }
    
    /// <summary>
    /// The logging level for database operations.
    /// </summary>
    public LogLevel LogLevel { get; internal set; } = LogLevel.Information;

    /// <summary>
    /// Creates a new database connection using the specified provider and connection string.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the provider factory is not available or the connection cannot be created.
    /// </exception>
    public IDbConnection CreateConnection()
    {
        var connection = Provider.Factory?.CreateConnection();
        if (connection is null)
            throw new InvalidOperationException(string.Format(Strings.FailedToCreateConnectionUsingProviderX, Provider.InvariantName));
        
        connection.ConnectionString = ConnectionString;
        return connection;
    }

    /// <summary>
    /// Copies the current connection options to another instance of <see cref="DbConnectionOptions"/>.
    /// </summary>
    /// <param name="other">
    /// The other instance of <see cref="DbConnectionOptions"/> to copy to.
    /// </param>
    public void CopyTo(DbConnectionOptions other)
    {
        other.ConnectionKey = ConnectionKey;
        other.Provider = Provider;
        other.ConnectionString = ConnectionString;
        other.Default = Default;
        other.Conventions = Conventions?.Clone();
        other.LogLevel = LogLevel;
    }

    /// <summary>
    /// Creates a clone of the current <see cref="DbConnectionOptions"/> instance.
    /// </summary>
    /// <returns>
    /// A new instance of <see cref="DbConnectionOptions"/> with the same properties as the current instance.
    /// </returns>
    public DbConnectionOptions Clone()
    {
        var other = new DbConnectionOptions(ConnectionKey);
        CopyTo(other);
        return other;
    }
}