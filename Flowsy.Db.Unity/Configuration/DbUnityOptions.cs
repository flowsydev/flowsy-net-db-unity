using Dapper;
using Flowsy.Db.Unity.Conventions;
using Flowsy.Db.Unity.Resources;
using Microsoft.Extensions.DependencyInjection;

namespace Flowsy.Db.Unity.Configuration;

/// <summary>
/// Configuration options for database conventions and services.
/// </summary>
public class DbUnityOptions
{
    private readonly Dictionary<string, DbConnectionOptionsBuilder> _connectionOptionsBuilders = new();

    internal DbUnityOptions()
    {
    }

    /// <summary>
    /// Allows to configure the default conventions for executing database queries and commands.
    /// These conventions will be used for all database connections unless overriden by specific options.
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
    /// All options defined for this connection will be available for dependency injection as a named IOptions{DbConnectionOptions} instance where the name will be the provided connection key.
    /// </summary>
    /// <code>
    /// // Inject IOptionsSnapshot{DbConnectionOptions} to resolve options by connection key
    /// var options = optionsSnapshot.Get("MyConnectionKey");
    /// var connectionString = options.ConnectionString;
    /// var conventions = options.Conventions;
    /// </code>
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
    
    /// <summary>
    /// Builds the options for all configured database connections.
    /// </summary>
    /// <returns></returns>
    internal IEnumerable<DbConnectionOptions> BuildConnectionOptions() =>
        _connectionOptionsBuilders.Values.Select(b => b.Build()).ToList();
}