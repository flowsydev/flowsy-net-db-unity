using System.Data;
using Flowsy.Db.Unity.Conventions;
using Flowsy.Db.Unity.Resources;

namespace Flowsy.Db.Unity;

public class DbConnectionOptions
{
    public DbConnectionOptions()
    {
    }
    
    public DbConnectionOptions(string connectionKey)
    {
        ConnectionKey = connectionKey;
    }

    public string ConnectionKey { get; private set; } = string.Empty;
    
    public DbProvider Provider { get; internal set; } = DbProvider.Generic;
    
    public string ConnectionString { get; internal set; } = string.Empty;
    
    public bool Default { get; internal set; }
    
    public Type ConnectionFactoryType { get; internal set; } = typeof(DbConnectionFactory);
    
    public Type AgentType { get; internal set; } = typeof(DbAgent);
    
    public Type UnitOfWorkType { get; internal set; } = typeof(DbUnitOfWork);
    
    public DbConventionSet? Conventions { get; internal set; }

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
        other.ConnectionFactoryType = ConnectionFactoryType;
        other.AgentType = AgentType;
        other.UnitOfWorkType = UnitOfWorkType;
    }
}