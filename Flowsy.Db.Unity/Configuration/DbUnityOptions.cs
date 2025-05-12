using Dapper;
using Flowsy.Core;
using Flowsy.Db.Unity.Conventions;
using Flowsy.Db.Unity.Resources;
using Microsoft.Extensions.DependencyInjection;

namespace Flowsy.Db.Unity.Configuration;

public class DbUnityOptions
{
    private readonly IServiceCollection _services;
    
    private readonly Dictionary<string, DbConnectionOptionsBuilder> _connectionOptionsBuilders = new();

    internal DbUnityOptions(IServiceCollection services)
    {
        _services = services;
    }

    public DbUnityOptions UseDefaultConventions(Action<DbConventionSetBuilder> configure)
    {
        var builder = new DbConventionSetBuilder(DbConventionSet.Default.Clone());
        configure(builder);
        DbConventionSet.Default = builder.Build();

        return this;
    }

    public DbConnectionOptionsBuilder UseConnection(string connectionKey)
    {
        if (string.IsNullOrWhiteSpace(connectionKey))
            throw new ArgumentException(Strings.ConnectionKeyCannotBeNullOrWhiteSpace, nameof(connectionKey));
        
        var options = new DbConnectionOptions(connectionKey);
        var builder = new DbConnectionOptionsBuilder(options);
        _connectionOptionsBuilders[connectionKey] = builder;
        return builder;
    }
    
    public DbUnityOptions MapTypes(Action<DbConventionTypeMapOptions> configure)
    {
        var options = new DbConventionTypeMapOptions();
        configure(options);
        
        foreach (var group in options.TypeGroups.Values)
        {
            foreach (var type in group.Types)
            {
                SqlMapper.RemoveTypeMap(type);
                SqlMapper.SetTypeMap(type, new DbConventionTypeMap(type, group.ColumnNaming, options.StrictMode));
            }
        }

        return this;
    }

    internal void RegisterServices()
    {
        _services.AddScoped<IDbConnectionScope, DbConnectionScope>();

        var unnamedOptionsRegistered = false;
        var index = -1;
        foreach (var (connectionKey, builder) in _connectionOptionsBuilders)
        {
            index++;
            var connectionOptions = builder.Build();
            
            if (!unnamedOptionsRegistered && (connectionOptions.Default || index == 0))
            {
                _services.AddOptions<DbConnectionOptions>().Configure(options =>
                {
                    connectionOptions.CopyTo(options);
                });
                unnamedOptionsRegistered = true;
            }
            _services.AddOptions<DbConnectionOptions>(connectionKey).Configure(options =>
            {
                connectionOptions.CopyTo(options);
            });
        }
    }
}