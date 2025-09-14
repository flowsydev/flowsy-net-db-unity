using System.Collections.Concurrent;
using System.Data;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Flowsy.Db.Unity.Test.Mock.Infrastructure.Database;

public class DbExtendedConnectionFactory : DbConnectionFactory, IDisposable
{
    private readonly ConcurrentDictionary<string, NpgsqlDataSource> _dataSources = new();
    private bool _disposed;
    
    public DbExtendedConnectionFactory(IOptionsMonitor<DbConnectionConfiguration> optionsMonitor) : base(optionsMonitor)
    {
    }

    ~DbExtendedConnectionFactory()
    {
        Dispose(false);
    }

    public override IDbConnection GetConnection(string? connectionKey = null, bool open = false)
    {
        var configuration = GetConfiguration(connectionKey);

        if (configuration.Provider.Family != DbProviderFamily.Postgres)
            return base.GetConnection(connectionKey, open);
        
        var connectionString = configuration.ConnectionString;
        _dataSources.TryGetValue(connectionString, out var dataSource);

        if (dataSource is null)
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

            var enumMappings = (
                from m in configuration.Conventions.Enums.Mappings
                where m.DatabaseTypeName is not null
                select m
            );
            foreach (var enumMapping in enumMappings)
            {
                dataSourceBuilder.MapEnum(
                    enumMapping.RuntimeType,
                    enumMapping.DatabaseTypeName!,
                    new DbPostgresEnumNameTranslator(enumMapping, configuration.Conventions.Enums.NameTranslator)
                    );
            }
            
            _dataSources[connectionString] = dataSource = dataSourceBuilder.Build();
        }

        var postgresConnection = dataSource.CreateConnection();
        
        if (open && postgresConnection.State == ConnectionState.Closed)
            postgresConnection.Open();
        
        return postgresConnection;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed || !disposing) return;
        
        // Liberar todos los NpgsqlDataSource cuando se dispone el factory
        foreach (var dataSource in _dataSources.Values)
        {
            dataSource.Dispose();
        }
        _dataSources.Clear();
        _disposed = true;
    }
}