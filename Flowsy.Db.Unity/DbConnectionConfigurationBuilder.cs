using System.Data.Common;
using Flowsy.Db.Unity.Conventions;
using Flowsy.Db.Unity.Resources;
using Microsoft.Extensions.Logging;

namespace Flowsy.Db.Unity;

/// <summary>
/// Provides a fluent way to configure a database connection.
/// </summary>
public class DbConnectionConfigurationBuilder
{
    /// <summary>
    /// The unique key to identify this connection.
    /// </summary>
    internal string ConnectionKey { get; }
    
    private readonly string _connectionString;
    private DbProviderDescriptor? _provider;
    private DbMigrationConfiguration? _migrationConfiguration;
    private DbConventionSetBuilder? _conventionSetBuilder;

    /// <summary>
    /// Indicates whether this connection is the default connection.
    /// </summary>
    internal bool Default { get; private set; }
    
    private LogLevel _logLevel = LogLevel.Debug;
    
    /// <summary>
    /// Indicates whether the current configuration is valid.
    /// </summary>
    internal bool IsValid => 
        !string.IsNullOrEmpty(ConnectionKey) && 
        !string.IsNullOrEmpty(_connectionString) && 
        _provider is not null;
    
    /// <summary>
    /// Creates a new instance of the <see cref="DbConnectionConfigurationBuilder"/> class.
    /// </summary>
    /// <param name="connectionKey">
    /// The unique key to identify this connection.
    /// </param>
    /// <param name="connectionString">
    /// The connection string to the database.
    /// </param>
    internal DbConnectionConfigurationBuilder(string connectionKey, string connectionString)
    {
        ConnectionKey = connectionKey;
        _connectionString = connectionString;
    }
    
    /// <summary>
    /// Marks this connection as the default connection.
    /// </summary>
    /// <param name="value">
    /// Indicates whether this connection should be the default. The default value is true.
    /// </param>
    /// <returns>
    /// The same <see cref="DbConnectionConfigurationBuilder"/> to allow for method chaining.
    /// </returns>
    public DbConnectionConfigurationBuilder AsDefault(bool value = true)
    {
        Default = value;
        return this;
    }
    
    /// <summary>
    /// Specifies the database provider to use for this connection.
    /// </summary>
    /// <param name="family">
    /// The family of the database provider.
    /// </param>
    /// <param name="invariantName">
    /// The invariant name of the database provider.
    /// </param>
    /// <param name="factory">
    /// The factory for the database provider.
    /// </param>
    /// <returns>
    /// The same <see cref="DbConnectionConfigurationBuilder"/> to allow for method chaining.
    /// </returns>
    public DbConnectionConfigurationBuilder WithProvider(DbProviderFamily family, string invariantName, DbProviderFactory factory)
    {
        _provider = new  DbProviderDescriptor(family, invariantName, factory);
        return this;
    }
    
    /// <summary>
    /// Specifies the logging level for operations on this connection.
    /// </summary>
    /// <param name="logLevel">
    /// The logging level to use.
    /// </param>
    /// <returns>
    /// The same <see cref="DbConnectionConfigurationBuilder"/> to allow for method chaining.
    /// </returns>
    public DbConnectionConfigurationBuilder WithLogLevel(LogLevel logLevel)
    {
        _logLevel = logLevel;
        return this;
    }

    /// <summary>
    /// Specifies the migration configuration for this connection.
    /// </summary>
    /// <param name="migrationScriptPath">
    /// The path where the migration scripts are located.
    /// </param>
    /// <param name="preMigrationScript">
    /// The script that is executed before applying the migrations.
    /// </param>
    /// <param name="postMigrationScript">
    /// The script that is executed after applying the migrations.
    /// </param>
    /// <param name="historyTableName">
    /// The name of the table where the migration history is stored.
    /// </param>
    /// <param name="historySchemaName">
    /// The schema where the migration history table is located.
    /// </param>
    /// <param name="outOfOrder">
    /// Indicates whether out-of-order migrations are allowed.
    /// For example, if OutOfOrder is true, if migration 2 has already been applied and migration 1 is added, applying it is allowed.
    /// </param>
    /// <returns>
    /// The same <see cref="DbConnectionConfigurationBuilder"/> to allow for method chaining.
    /// </returns>
    public DbConnectionConfigurationBuilder WithMigrations(
        string migrationScriptPath,
        string? preMigrationScript = null,
        string? postMigrationScript = null,
        string? historyTableName = null,
        string? historySchemaName = null,
        bool outOfOrder = false
        )
        => WithMigrations(new DbMigrationConfiguration(
            migrationScriptPath,
            preMigrationScript,
            postMigrationScript,
            historyTableName,
            historySchemaName,
            outOfOrder
            ));

    /// <summary>
    /// Specifies the migration configuration for this connection.
    /// </summary>
    /// <param name="configuration">
    /// The migration configuration to use.
    /// </param>
    /// <returns>
    /// The same <see cref="DbConnectionConfigurationBuilder"/> to allow for method chaining.
    /// </returns>
    public DbConnectionConfigurationBuilder WithMigrations(DbMigrationConfiguration configuration)
    {
        _migrationConfiguration = configuration;
        return this;
    }

    /// <summary>
    /// Provides access to the convention set builder to define the conventions to be used in this connection.
    /// </summary>
    /// <returns>
    /// An instance of <see cref="DbConventionSetBuilder"/> to define the conventions.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Throws an exception if the database provider is not set.
    /// </exception>
    public DbConventionSetBuilder WithConventions()
    {
        if (_conventionSetBuilder is not null)
            return _conventionSetBuilder;
        
        if (_provider is null)
            throw new InvalidOperationException(Strings.DatabaseProviderMustBeSet);
        
        _conventionSetBuilder = new DbConventionSetBuilder(_provider);
        return _conventionSetBuilder;
    }
    
    /// <summary>
    /// Builds the connection configuration and applies it to the provided instance of <see cref="DbConnectionConfiguration"/>.
    /// </summary>
    /// <param name="configuration">
    /// The instance of <see cref="DbConnectionConfiguration"/> where the built configuration will be applied.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Throws an exception if the connection key, connection string, or provider are not set.
    /// </exception>
    internal void Build(DbConnectionConfiguration configuration)
    {
        if (string.IsNullOrEmpty(ConnectionKey))
            throw new InvalidOperationException(Strings.ConnectionKeyMustBeSet);
        
        if (string.IsNullOrEmpty(_connectionString))
            throw new InvalidOperationException(Strings.ConnectionStringMustBeSet);
        
        if (_provider is null)
            throw new InvalidOperationException(Strings.DatabaseProviderMustBeSet);
        
        configuration.ConnectionKey = ConnectionKey;
        configuration.ConnectionString = _connectionString;
        configuration.Provider = _provider;
        configuration.Default = Default;
        configuration.LogLevel = _logLevel;
        configuration.Migrations = _migrationConfiguration;
        configuration.Conventions = _conventionSetBuilder?.Build() ?? DbConventionSet.Default;
    }
}