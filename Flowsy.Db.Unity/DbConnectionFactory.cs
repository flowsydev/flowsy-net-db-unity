using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using Flowsy.Db.Unity.Resources;
using Microsoft.Extensions.Options;

namespace Flowsy.Db.Unity;

/// <summary>
/// Represents a database connection factory.
/// </summary>
public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly IOptionsMonitor<DbConnectionConfiguration>? _optionsMonitor;
    private readonly ConcurrentDictionary<string, DbConnectionConfiguration> _configurations = [];

    /// <summary>
    /// Creates a new instance of the DbConnectionFactory class.
    /// </summary>
    /// <param name="configurations">
    /// An array of <see cref="DbConnectionConfiguration"/> to register with the factory.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Throws an exception if no connection configurations are provided.
    /// </exception>
    public DbConnectionFactory(params DbConnectionConfiguration[] configurations)
    {
        if (configurations.Length == 0)
            throw new InvalidOperationException(Strings.NoConnectionConfigurationsProvided);
        
        foreach (var config in configurations)
            _configurations[config.ConnectionKey] = config;
    }

    /// <summary>
    /// Creates a new instance of the DbConnectionFactory class.
    /// </summary>
    /// <param name="optionsMonitor">
    /// An IOptionsMonitor of DbConnectionConfiguration to register with the factory.
    /// </param>
    public DbConnectionFactory(IOptionsMonitor<DbConnectionConfiguration> optionsMonitor)
    {
        _optionsMonitor = optionsMonitor;
    }

    /// <summary>
    /// Gets the default connection key used by the factory.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Throws an exception if no default connection configuration is found.
    /// </exception>
    public virtual string DefaultConnectionKey
    {
        get
        {
            if (_optionsMonitor != null)
                return _optionsMonitor.CurrentValue.ConnectionKey;
            
            return _configurations.Values.FirstOrDefault(c => c.Default)?.ConnectionKey 
                   ?? throw new InvalidOperationException(Strings.NoDefaultConnectionConfigurationFound);
        }
    }

    /// <summary>
    /// Checks if a configuration exists for a specific connection key.
    /// If no key is provided, the default connection key is used.
    /// </summary>
    /// <param name="connectionKey">
    /// The connection key to check for configuration.
    /// </param>
    /// <returns>
    /// True if a configuration exists for the specified key; otherwise, false.
    /// </returns>
    public bool HasConfiguration(string? connectionKey = null)
        => TryGetConfiguration(connectionKey, out _);

    /// <summary>
    /// Tries to get the connection configuration for a specific connection key.
    /// If no key is provided, the default connection key is used.
    /// </summary>
    /// <param name="connectionKey">
    /// The connection key for which the configuration is desired.
    /// </param>
    /// <param name="configuration">
    /// When this method returns, contains the connection configuration associated with the specified key,
    /// if the key is found; otherwise, null. This parameter is passed uninitialized.
    /// </param>
    /// <returns>
    /// True if the connection configuration was found; otherwise, false.
    /// </returns>
    public virtual bool TryGetConfiguration(string? connectionKey, [MaybeNullWhen(false)] out DbConnectionConfiguration configuration)
    {
        configuration = null;
        
        var config = _optionsMonitor?.Get(connectionKey);
        if (config is not null && (string.IsNullOrEmpty(connectionKey) || config.ConnectionKey == connectionKey))
        {
            configuration = config;
            return true;
        }

        configuration = !string.IsNullOrEmpty(connectionKey)
            ? _configurations.GetValueOrDefault(connectionKey) 
            : _configurations.Values.FirstOrDefault(c => c.Default);
        
        return configuration is not null;
    }

    /// <summary>
    /// Gets the connection configuration for a specific connection key.
    /// If no key is provided, the default connection key is used.
    /// </summary>
    /// <param name="connectionKey">
    /// The connection key for which the configuration is desired.
    /// </param>
    /// <returns>
    /// An instance of <see cref="DbConnectionConfiguration"/> that contains the connection configuration.
    /// </returns>
    public virtual DbConnectionConfiguration GetConfiguration(string? connectionKey = null)
    {
        var configuration = _optionsMonitor?.Get(connectionKey);
        if (configuration is not null)
            return configuration;

        if (!string.IsNullOrEmpty(connectionKey))
            return _configurations.TryGetValue(connectionKey, out configuration)
                ? configuration
                : throw new KeyNotFoundException(string.Format(Strings.ConfigurationNotFoundForConnectionIdentifiedByX, connectionKey));
        
        configuration = _configurations.Values.FirstOrDefault(c => c.Default);
        return configuration ?? throw new InvalidOperationException(Strings.CouldNotResolveDefaultConnectionConfiguration);

    }

    /// <summary>
    /// Tries to get a database connection using the default connection key.
    /// </summary>
    /// <param name="connection">
    /// When this method returns, contains a database connection associated with the default connection key,
    /// if the connection was successfully created; otherwise, null.
    /// </param>
    /// <returns>
    /// True if the connection was successfully created; otherwise, false.
    /// </returns>
    public bool TryGetConnection([MaybeNullWhen(false)] out IDbConnection connection)
        => TryGetConnection(DefaultConnectionKey, false, out connection);

    /// <summary>
    /// Tries to get a database connection using the default connection key.
    /// </summary>
    /// <param name="open">
    /// Indicates whether the connection should be opened immediately after being created.
    /// </param>
    /// <param name="connection">
    /// When this method returns, contains a database connection associated with the default connection key,
    /// if the connection was successfully created; otherwise, null.
    /// </param>
    /// <returns>
    /// True if the connection was successfully created; otherwise, false.
    /// </returns>
    public bool TryGetConnection(bool open, [MaybeNullWhen(false)] out IDbConnection connection)
        => TryGetConnection(DefaultConnectionKey, open, out connection);

    /// <summary>
    /// Tries to get a database connection using the specified connection key.
    /// </summary>
    /// <param name="connectionKey">
    /// The connection key for which the connection is desired.
    /// </param>
    /// <param name="connection">
    /// When this method returns, contains a database connection associated with the specified connection key,
    /// if the connection was successfully created; otherwise, null.
    /// </param>
    /// <returns>
    /// True if the connection was successfully created; otherwise, false.
    /// </returns>
    public bool TryGetConnection(string connectionKey, [MaybeNullWhen(false)] out IDbConnection connection)
        => TryGetConnection(connectionKey, false, out connection);

    /// <summary>
    /// Tries to get a database connection using the specified connection key.
    /// </summary>
    /// <param name="connectionKey">
    /// The connection key for which the connection is desired.
    /// </param>
    /// <param name="open">
    /// Indicates whether the connection should be opened immediately after being created.
    /// </param>
    /// <param name="connection">
    /// When this method returns, contains a database connection associated with the specified connection key,
    /// if the connection was successfully created; otherwise, null.
    /// </param>
    /// <returns>
    /// True if the connection was successfully created; otherwise, false.
    /// </returns>
    public bool TryGetConnection(string connectionKey, bool open, [MaybeNullWhen(false)] out IDbConnection connection)
    {
        connection = null;
        
        if (!TryGetConfiguration(connectionKey, out var configuration))
            return false;

        try
        {
            var provider = configuration.Provider;
            connection = provider.Factory?.CreateConnection();
            if (connection == null)
                return false;
        
            connection.ConnectionString = configuration.ConnectionString;
            if (open && connection.State == ConnectionState.Closed)
                connection.Open();
        
            return true;
        }
        catch
        {
            connection = null;
            return false;
        }
    }

    /// <summary>
    /// Gets a database connection using the specified connection key.
    /// If no key is provided, the default connection key is used.
    /// </summary>
    /// <param name="connectionKey">
    /// The connection key for which the connection is desired.
    /// </param>
    /// <param name="open">
    /// Indicates whether the connection should be opened immediately after being created.
    /// </param>
    /// <returns>
    /// An instance of <see cref="IDbConnection"/> that represents the database connection.
    /// </returns>
    public virtual IDbConnection GetConnection(string? connectionKey = null, bool open = false)
    {
        var configuration = GetConfiguration(connectionKey);
        
        var provider = configuration.Provider;
        var connection = provider.Factory?.CreateConnection();
        if (connection == null)
            throw new InvalidOperationException(string.Format(Strings.FailedToCreateConnectionForProviderX, provider.InvariantName ?? provider.Family.ToString()));
        
        connection.ConnectionString = configuration.ConnectionString;
        
        if (open && connection.State == ConnectionState.Closed)
            connection.Open();
        
        return connection;
    }
}