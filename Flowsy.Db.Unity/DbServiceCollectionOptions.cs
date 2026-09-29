using Flowsy.Db.Unity.Conventions;
using Microsoft.Extensions.DependencyInjection;

namespace Flowsy.Db.Unity;

/// <summary>
/// Provides configuration for the database services collection.
/// </summary>
public class DbServiceCollectionOptions
{
    private readonly IServiceCollection _services;
    
    private readonly List<DbConnectionConfigurationBuilder> _connectionConfigurationBuilders = [];
    internal IEnumerable<DbConnectionConfigurationBuilder> ConnectionConfigurationBuilders => _connectionConfigurationBuilders;
    
    internal bool ConnectionFactoryRegistered { get; private set; }
    internal bool SessionFactoryRegistered { get; private set; }
    internal IList<Action> GlobalTypeHandlerRegistrations { get; } = [];
    private readonly HashSet<Type> _connectionProviderTypes = [];

    internal DbServiceCollectionOptions(IServiceCollection services)
    {
        _services = services;
    }

    /// <summary>
    /// Adds a new database connection configuration to the service collection.
    /// </summary>
    /// <param name="connectionKey">
    /// The unique key that identifies the database connection.
    /// </param>
    /// <param name="connectionString">
    /// The connection string for the database.
    /// </param>
    /// <returns>
    /// An instance of <see cref="DbConnectionConfigurationBuilder"/> to configure connection options.
    /// </returns>
    public DbConnectionConfigurationBuilder UseConnection(string connectionKey, string connectionString)
    {
        var connectionConfigurationBuilder = new DbConnectionConfigurationBuilder(connectionKey, connectionString);
        _connectionConfigurationBuilders.Add(connectionConfigurationBuilder);
        return connectionConfigurationBuilder;
    }

    /// <summary>Registers a connection creator supplied by a provider extension.</summary>
    public DbServiceCollectionOptions AddConnectionProvider<TProvider>()
        where TProvider : class, IDbConnectionProvider
    {
        if (_connectionProviderTypes.Add(typeof(TProvider)))
            _services.AddSingleton<IDbConnectionProvider, TProvider>();
        return this;
    }

    /// <summary>Registers a process-wide Dapper type handler.</summary>
    public DbServiceCollectionOptions AddGlobalTypeHandler<T>(Dapper.SqlMapper.TypeHandler<T> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        GlobalTypeHandlerRegistrations.Add(() => Dapper.SqlMapper.AddTypeHandler(handler));
        return this;
    }

    /// <summary>
    /// Adds a custom connection factory to the service collection.
    /// This allows creating connections using a custom factory capable of leveraging specific features of a database provider or particular environment.
    /// </summary>
    /// <typeparam name="TConnectionFactory">
    /// The type of the connection factory that implements the <see cref="IDbConnectionFactory"/> interface.
    /// </typeparam>
    /// <returns>
    /// The current instance of <see cref="DbServiceCollectionOptions"/> to allow fluent configuration.
    /// </returns>
    public DbServiceCollectionOptions UseConnectionFactory<TConnectionFactory>() where TConnectionFactory : class, IDbConnectionFactory
    {
        if (ConnectionFactoryRegistered)
            return this;
        
        _services.AddSingleton<IDbConnectionFactory, TConnectionFactory>();
        ConnectionFactoryRegistered = true;
        return this;
    }
    
    /// <summary>
    /// Adds a custom connection factory to the service collection.
    /// This allows creating database sessions using a custom factory capable of leveraging specific features of a database provider or particular environment.
    /// </summary>
    /// <param name="implementationFactory">
    /// A function to create the connection factory implementation.
    /// </param>
    /// <typeparam name="TConnectionFactory">
    /// The type of the connection factory that implements the <see cref="IDbConnectionFactory"/> interface.
    /// </typeparam>
    /// <returns>
    /// The current instance of <see cref="DbServiceCollectionOptions"/> to allow fluent configuration.
    /// </returns>
    public DbServiceCollectionOptions UseConnectionFactory<TConnectionFactory>(Func<IServiceProvider, TConnectionFactory> implementationFactory) where TConnectionFactory : class, IDbConnectionFactory
    {
        if (ConnectionFactoryRegistered)
            return this;
        
        _services.AddScoped<IDbConnectionFactory>(implementationFactory);
        ConnectionFactoryRegistered = true;
        return this;
    }
    
    /// <summary>
    /// Adds a custom session factory to the service collection.
    /// This allows creating database sessions using a custom factory capable of leveraging specific features of a database provider or particular environment.
    /// </summary>
    /// <typeparam name="TSessionFactory">
    /// The type of the session factory that implements the <see cref="IDbSessionFactory"/> interface.
    /// </typeparam>
    /// <returns>
    /// The current instance of <see cref="DbServiceCollectionOptions"/> to allow fluent configuration.
    /// </returns>
    public DbServiceCollectionOptions UseSessionFactory<TSessionFactory>() where TSessionFactory : class, IDbSessionFactory
    {
        if (SessionFactoryRegistered)
            return this;
        
        _services.AddScoped<IDbSessionFactory, TSessionFactory>();
        SessionFactoryRegistered = true;
        return this;
    }

    /// <summary>
    /// Adds a custom session factory to the service collection.
    /// This allows creating database sessions using a custom factory capable of leveraging specific features of a database provider or particular environment.
    /// </summary>
    /// <param name="implementationFactory">
    /// A function to create the session factory implementation.
    /// </param>
    /// <typeparam name="TSessionFactory">
    /// The type of the session factory that implements the <see cref="IDbSessionFactory"/> interface.
    /// </typeparam>
    /// <returns>
    /// The current instance of <see cref="DbServiceCollectionOptions"/> to allow fluent configuration.
    /// </returns>
    public DbServiceCollectionOptions UseSessionFactory<TSessionFactory>(Func<IServiceProvider, TSessionFactory> implementationFactory) where TSessionFactory : class, IDbSessionFactory
    {
        if (SessionFactoryRegistered)
            return this;
        
        _services.AddScoped<IDbSessionFactory>(implementationFactory);
        SessionFactoryRegistered = true;
        return this;
    }
    
    /// <summary>
    /// Registers type mappings for database objects with the specified naming conventions.
    /// This allows automatic mapping of CLR types to database columns using the specified case style and optional prefix/suffix.
    /// </summary>
    /// <param name="caseStyle">
    /// The case style to apply to column names when mapping types.
    /// </param>
    /// <param name="prefix">
    /// Optional prefix to add to column names.
    /// </param>
    /// <param name="suffix">
    /// Optional suffix to add to column names.
    /// </param>
    /// <param name="strictMode">
    /// If <c>true</c>, throws exceptions when mappings cannot be found.
    /// If <c>false</c>, returns <c>null</c> for unmappable members.
    /// </param>
    /// <param name="types">
    /// The types to register for automatic mapping.
    /// </param>
    /// <returns>
    /// The current instance of <see cref="DbServiceCollectionOptions"/> to allow fluent configuration.
    /// </returns>
    public DbServiceCollectionOptions MapTypes(DbCaseStyle caseStyle, string? prefix = null, string? suffix = null, bool strictMode = false, params Type[] types)
        => MapTypes(new DbObjectConvention(caseStyle, prefix, suffix), strictMode, types);
    
    /// <summary>
    /// Registers type mappings for database objects with the specified object convention.
    /// This allows automatic mapping of CLR types to database columns using the specified convention.
    /// </summary>
    /// <param name="columnConvention">
    /// The naming convention to apply to column names when mapping types.
    /// </param>
    /// <param name="strictMode">
    /// If <c>true</c>, throws exceptions when mappings cannot be found.
    /// If <c>false</c>, returns <c>null</c> for unmappable members.
    /// </param>
    /// <param name="types">
    /// The types to register for automatic mapping.
    /// </param>
    /// <returns>
    /// The current instance of <see cref="DbServiceCollectionOptions"/> to allow fluent configuration.
    /// </returns>
    public DbServiceCollectionOptions MapTypes(DbObjectConvention columnConvention, bool strictMode, params Type[] types)
    {
        foreach (var type in types)
            DbConventionTypeMap.Register(type, columnConvention, strictMode);
        
        return this;
    }
}
