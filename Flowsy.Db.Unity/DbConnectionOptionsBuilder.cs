using System.Data.Common;
using Flowsy.Db.Unity.Conventions;
using Flowsy.Db.Unity.Resources;
using Microsoft.Extensions.Logging;

namespace Flowsy.Db.Unity;

/// <summary>
/// Builder for configuring database connection options.
/// </summary>
public class DbConnectionOptionsBuilder
{
    private readonly DbConnectionOptions _options;
    private DbConventionSetBuilder _conventionSetBuilder;
    private bool _providerSet;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="DbConnectionOptionsBuilder"/> class.
    /// </summary>
    /// <param name="connectionKey">
    /// The unique key for the database connection.
    /// </param>
    public DbConnectionOptionsBuilder(string connectionKey) : this(new DbConnectionOptions(connectionKey))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DbConnectionOptionsBuilder"/> class.
    /// </summary>
    /// <param name="options"></param>
    public DbConnectionOptionsBuilder(DbConnectionOptions options)
    {
        _options = options;
        _conventionSetBuilder = new DbConventionSetBuilder(options.Provider);
        _providerSet = true;
    }

    /// <summary>
    /// Allows to set the connection as default.
    /// The default connection will be used by the default IDbAgent and IDbUnitOfWork instances.
    /// </summary>
    /// <param name="default">
    /// true to set the connection as default; otherwise, false.
    /// </param>
    /// <returns>
    /// The current instance of <see cref="DbConnectionOptionsBuilder"/>.
    /// </returns>
    public DbConnectionOptionsBuilder AsDefault(bool @default = true)
    {
        _options.Default = @default;
        return this;
    }

    /// <summary>
    /// Allows to configure the database provider for the connection.
    /// </summary>
    /// <param name="provider">
    /// The database provider descriptor.
    /// </param>
    /// <returns>
    /// The current instance of <see cref="DbConnectionOptionsBuilder"/>.
    /// </returns>
    public DbConnectionOptionsBuilder WithProvider(DbProviderDescriptor provider)
    {
        _options.Provider = provider;
        _conventionSetBuilder = new DbConventionSetBuilder(provider);
        _providerSet = true;
        return this;
    }

    /// <summary>
    /// Allows to configure the database provider for the connection.
    /// </summary>
    /// <param name="family">
    /// The database provider family.
    /// </param>
    /// <param name="invariantName">
    /// The invariant name of the database provider.
    /// </param>
    /// <param name="factory">
    /// The database provider factory.
    /// </param>
    /// <returns></returns>
    public DbConnectionOptionsBuilder WithProvider(DbProviderFamily family, string invariantName, DbProviderFactory factory)
    {
        _options.Provider = new DbProviderDescriptor(family, invariantName, factory);
        _conventionSetBuilder = new DbConventionSetBuilder(_options.Provider);
        _providerSet = true;
        return this;
    }
    
    /// <summary>
    /// Sets the connection string for the database connection.
    /// </summary>
    /// <param name="connectionString">
    /// The connection string to be used for the database connection.
    /// </param>
    /// <returns></returns>
    public DbConnectionOptionsBuilder WithConnectionString(string connectionString)
    {
        _options.ConnectionString = connectionString;
        return this;
    }
    
    /// <summary>
    /// Sets the log level for the operations peformed on the connection.
    /// </summary>
    /// <param name="logLevel">
    /// The log level to be used for the database connection.
    /// </param>
    /// <returns>
    /// The current instance of <see cref="DbConnectionOptionsBuilder"/>.
    /// </returns>
    public DbConnectionOptionsBuilder WithLogLevel(LogLevel logLevel)
    {
        _options.LogLevel = logLevel;
        return this;
    }

    /// <summary>
    /// Allows to configure the conventions for the database connection.
    /// A set of conventions defines naming preferences and other options to be used when executing database queries and commands.
    /// </summary>
    /// <param name="conventions">
    /// The conventions to be used for the database connection.
    /// </param>
    /// <returns></returns>
    public DbConnectionOptionsBuilder WithConventions(DbConventionSet conventions)
    {
        _conventionSetBuilder = new DbConventionSetBuilder(conventions);
        _options.Provider = _conventionSetBuilder.Conventions.Provider;
        _providerSet = true;
        return this;
    }

    /// <summary>
    /// Allows to configure the conventions for the database connection.
    /// A set of conventions defines naming preferences and other options to be used when executing database queries and commands.
    /// </summary>
    /// <returns>
    /// A <see cref="DbConventionSetBuilder"/> instance to configure the conventions.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the provider is not set before configuring conventions.
    /// </exception>
    public DbConventionSetBuilder WithConventions()
    {
        if (!_providerSet)
            throw new InvalidOperationException(Strings.ProviderMustBeSetBeforeConfiguringConventions);
        
        return _conventionSetBuilder;
    }

    /// <summary>
    /// Builds the <see cref="DbConnectionOptions"/> instance with the configured options.
    /// </summary>
    /// <returns>
    /// The configured <see cref="DbConnectionOptions"/> instance.
    /// </returns>
    public DbConnectionOptions Build()
    {
        _options.Conventions = _conventionSetBuilder.Build();
        return _options;
    }
}