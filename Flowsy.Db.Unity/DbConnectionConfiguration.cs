using System.Data;
using Flowsy.Db.Unity.Conventions;
using Flowsy.Db.Unity.Resources;
using Microsoft.Extensions.Logging;

namespace Flowsy.Db.Unity;

/// <summary>
/// Represents the connection configuration to a database.
/// </summary>
public record DbConnectionConfiguration
{
    private IReadOnlyDictionary<Type, IDbProviderConfiguration> _providerConfigurations =
        new Dictionary<Type, IDbProviderConfiguration>();
    /// <summary>
    /// Initializes a new instance of the <see cref="DbConnectionConfiguration"/> class with default values.
    /// </summary>
    public DbConnectionConfiguration() : this(string.Empty, string.Empty, DbProviderDescriptor.Generic)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DbConnectionConfiguration"/> class.
    /// </summary>
    /// <param name="connectionKey">
    /// Unique key to identify the database connection.
    /// This key is used to request connections from services like <see cref="IDbConnectionFactory"/> or <see cref="IDbConnectionHub"/>.
    /// </param>
    /// <param name="connectionString">
    /// Connection string for the database.
    /// </param>
    /// <param name="provider">
    /// Database provider information.
    /// </param>
    /// <param name="default">
    /// Indicates whether this connection is the default connection.
    /// If the value is <c>true</c>, this connection will be used as the default connection for database operations when none is specified.
    /// </param>
    /// <param name="logLevel">
    /// Logging level for database operations.
    /// </param>
    public DbConnectionConfiguration(
        string connectionKey,
        string connectionString,
        DbProviderDescriptor provider,
        bool @default = false,
        LogLevel logLevel = LogLevel.Debug
        )
    {
        ConnectionKey = connectionKey;
        ConnectionString = connectionString;
        Provider = provider;
        Default = @default;
        Conventions = DbConventionSet.Default;
        LogLevel = logLevel;
    }

    /// <summary>
    /// Unique key to identify the database connection.
    /// This key is used to request connections from services like <see cref="IDbConnectionFactory"/> or <see cref="IDbConnectionHub"/>.
    /// </summary>
    public string ConnectionKey { get; internal set; }
    
    /// <summary>
    /// Connection string for the database.
    /// </summary>
    public string ConnectionString { get; internal set; }
    
    /// <summary>
    /// Database provider information.
    /// </summary>
    public DbProviderDescriptor Provider { get; internal set; }
    
    /// <summary>
    /// Indicates whether this connection is the default connection.
    /// If the value is <c>true</c>, this connection will be used as the default connection for database operations when none is specified.
    /// </summary>
    public bool Default { get; internal set; }
    
    /// <summary>
    /// Logging level for database operations.
    /// </summary>
    public LogLevel LogLevel { get; internal set; }
    
    /// <summary>
    /// Database conventions to be used for this connection.
    /// </summary>
    public DbConventionSet Conventions { get; internal set; }
    
    /// <summary>
    /// Migration configuration for this connection.
    /// </summary>
    public DbMigrationConfiguration? Migrations { get; internal set; }

    /// <summary>Gets whether detected write operations require an active transaction.</summary>
    public bool RequireTransactionForWrites { get; internal set; }

    /// <summary>Gets administrative statements exempted from the transaction guard.</summary>
    public IReadOnlySet<string> WriteTransactionExceptions { get; internal set; } =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    /// <summary>Gets additional session settings explicitly allowed for this connection.</summary>
    public IReadOnlySet<string> AllowedSessionSettings { get; internal set; } =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    /// <summary>Gets the optional threshold used to classify a database operation as slow.</summary>
    public TimeSpan? SlowOperationThreshold { get; internal set; }

    /// <summary>Gets configuration objects registered by provider extensions.</summary>
    public IReadOnlyDictionary<Type, IDbProviderConfiguration> ProviderConfigurations
    {
        get => _providerConfigurations;
        internal set => _providerConfigurations = value;
    }

    /// <summary>Tries to resolve configuration registered by a provider extension.</summary>
    public bool TryGetProviderConfiguration<TConfiguration>(out TConfiguration? configuration)
        where TConfiguration : class, IDbProviderConfiguration
    {
        configuration = ProviderConfigurations.GetValueOrDefault(typeof(TConfiguration)) as TConfiguration;
        return configuration is not null;
    }
    
    /// <summary>
    /// Creates a connection to the database using the current configuration.
    /// </summary>
    /// <param name="open">
    /// Indicates whether the connection should be opened immediately after being created.
    /// </param>
    /// <returns>
    /// An instance of <see cref="IDbConnection"/> configured with the specified connection string and provider.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Throws an exception if the provider defined by the <see cref="Provider"/> property does not have an available factory or if the connection cannot be created.
    /// </exception>
    public IDbConnection CreateConnection(bool open = false)
    {
        var connection = Provider.Factory?.CreateConnection();
        if (connection is null)
            throw new InvalidOperationException(string.Format(Strings.FailedToCreateConnectionForProviderX, Provider.InvariantName));
        
        connection.ConnectionString = ConnectionString;

        if (open && connection.State == ConnectionState.Closed)
            connection.Open();
        
        return connection;
    }
}
