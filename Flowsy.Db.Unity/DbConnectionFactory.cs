using System.Collections.Concurrent;
using System.Data;
using Flowsy.Db.Unity.Resources;
using Microsoft.Extensions.Options;

namespace Flowsy.Db.Unity;

/// <summary>
/// Obtains database connections based on the provided configuration.
/// Consumers of this service must dispose of the connections when no longer needed.
/// </summary>
public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly ConcurrentDictionary<string, DbConnectionOptions> _optionsDictionary = new();
    private readonly IOptionsSnapshot<DbConnectionOptions>? _optionsSnapshot;

    protected DbConnectionFactory()
    {
    }
    
    /// <summary>
    /// Creates a new instance of the DbConnectionFactory class.
    /// </summary>
    /// <param name="optionsList">
    /// A list of DbConnectionOptions to register with the factory.
    /// </param>
    public DbConnectionFactory(IEnumerable<DbConnectionOptions> optionsList)
    {
        foreach (var options in optionsList)
            RegisterOptions(options);
    }
    
    /// <summary>
    /// Creates a new instance of the DbConnectionFactory class.
    /// </summary>
    /// <param name="optionsSnapshot">
    /// An IOptionsSnapshot of DbConnectionOptions to register with the factory.
    /// Each named option is expected to have a name matching the connection key of its corresponding DbConnectionOptions instance.
    /// </param>
    public DbConnectionFactory(IOptionsSnapshot<DbConnectionOptions> optionsSnapshot)
    {
        _optionsSnapshot = optionsSnapshot;
    }

    private void RegisterOptions(DbConnectionOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ConnectionKey))
            throw new InvalidOperationException(Strings.ConnectionKeyCannotBeNullOrWhiteSpace);
        
        if (_optionsDictionary.ContainsKey(options.ConnectionKey))
            throw new InvalidOperationException(string.Format(Strings.ConnecionKeyXAlreadyExists, options.ConnectionKey));
            
        if (string.IsNullOrWhiteSpace(options.ConnectionString))
            throw new InvalidOperationException(Strings.ConnectionStringCannotBeNullOrWhiteSpace);
        
        _optionsDictionary[options.ConnectionKey] = options;
    }

    /// <summary>
    /// Obtains the DbConnectionOptions for the specified connection key.
    /// </summary>
    /// <param name="connectionKey">
    /// The key that identifies the configuration to use to create the connection.
    /// </param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the connection key is not found or does not match the expected key.
    /// </exception>
    protected DbConnectionOptions GetConnectionOptions(string connectionKey)
    {
        _optionsDictionary.TryGetValue(connectionKey, out var options);

        if (options is null && _optionsSnapshot is not null)
            options = _optionsSnapshot.Get(connectionKey);
        
        if (options is null || options.ConnectionKey != connectionKey)
            throw new InvalidOperationException(string.Format(Strings.InvalidConnectionKeyX, connectionKey));
        
        return options;
    }

    /// <summary>
    /// Obtains a database connection using the DbConnectionOptions identified by the provided connection key.
    /// </summary>
    /// <param name="connectionKey">
    /// The key that identifies the configuration to use to create the connection.
    /// </param>
    /// <param name="open">
    /// A value indicating whether the connection should be opened.
    /// </param>
    /// <returns>A database connection.</returns>
    public virtual IDbConnection GetConnection(string connectionKey, bool open = false)
    {
        var options = GetConnectionOptions(connectionKey);

        var provider = options.Provider;
        var connection = provider.Factory?.CreateConnection();
        if (connection == null)
            throw new InvalidOperationException(string.Format(Strings.FailedToCreateConnectionUsingProviderX, provider.InvariantName ?? provider.Family.ToString()));
        
        connection.ConnectionString = options.ConnectionString;
        
        if (open)
            connection.Open();
        
        return connection;
    }
}