using Dapper;
using Flowsy.Core;
using Flowsy.Db.Unity.Conventions;
using Flowsy.Db.Unity.Resources;
using Microsoft.Extensions.DependencyInjection;

namespace Flowsy.Db.Unity.Configuration;

/// <summary>
/// Configuration options for database conventions and services.
/// </summary>
public class DbUnityOptions
{
    private readonly IServiceCollection _services;
    
    private readonly Dictionary<string, DbConnectionOptionsBuilder> _connectionOptionsBuilders = new();

    internal DbUnityOptions(IServiceCollection services)
    {
        _services = services;
    }

    /// <summary>
    /// Allows to configure the default conventions for executing database queries and commands.
    /// </summary>
    /// <param name="configure">
    /// The action to configure the default conventions.
    /// </param>
    /// <returns>
    /// The current instance of <see cref="DbUnityOptions"/>.
    /// </returns>
    public DbUnityOptions UseDefaultConventions(Action<DbConventionSetBuilder> configure)
    {
        var builder = new DbConventionSetBuilder(DbConventionSet.Default.Clone());
        configure(builder);
        DbConventionSet.Default = builder.Build();

        return this;
    }

    /// <summary>
    /// Defines a key to uniquely identify a database connection.
    /// This connection can be associated with a set of options and conventions for database operations.
    /// </summary>
    /// <param name="connectionKey">
    /// The unique key for the database connection.
    /// </param>
    /// <returns>
    /// A <see cref="DbConnectionOptionsBuilder"/> instance to configure the connection options.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the <paramref name="connectionKey"/> is null or whitespace.
    /// </exception>
    public DbConnectionOptionsBuilder UseConnection(string connectionKey)
    {
        if (string.IsNullOrWhiteSpace(connectionKey))
            throw new ArgumentException(Strings.ConnectionKeyCannotBeNullOrWhiteSpace, nameof(connectionKey));
        
        var options = new DbConnectionOptions(connectionKey);
        var builder = new DbConnectionOptionsBuilder(options);
        _connectionOptionsBuilders[connectionKey] = builder;
        return builder;
    }
    
    /// <summary>
    /// Registers conventions for mapping database columns to type members when executing queries.
    /// </summary>
    /// <param name="configure">
    /// The action to configure the type mapping conventions.
    /// </param>
    /// <returns>
    /// The current instance of <see cref="DbUnityOptions"/>.
    /// </returns>
    public DbUnityOptions MapTypes(Action<DbConventionTypeMapOptions> configure)
    {
        var options = new DbConventionTypeMapOptions();
        configure(options);
        
        foreach (var group in options.TypeGroups)
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