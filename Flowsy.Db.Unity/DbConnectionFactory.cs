using System.Collections.Concurrent;
using System.Data;
using Flowsy.Db.Unity.Resources;
using Microsoft.Extensions.Options;

namespace Flowsy.Db.Unity;

public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly ConcurrentDictionary<string, DbConnectionOptions> _optionsDictionary = new();
    private readonly IOptionsSnapshot<DbConnectionOptions>? _optionsSnapshot;

    protected DbConnectionFactory()
    {
    }
    
    public DbConnectionFactory(IEnumerable<DbConnectionOptions> optionsList)
    {
        foreach (var options in optionsList)
            RegisterOptions(options);
    }
    
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
    /// Obtains a database connection using the DbConnectionOptions identified by the provided connection key.
    /// </summary>
    /// <param name="connectionKey">
    /// The key that identifies the configuration to use to create the connection.
    /// </param>
    /// <param name="open">
    /// A value indicating whether the connection should be opened.
    /// </param>
    /// <returns>A database connection</returns>
    public IDbConnection GetConnection(string connectionKey, bool open = false)
    {
        _optionsDictionary.TryGetValue(connectionKey, out var options);

        if (options is null && _optionsSnapshot is not null)
            options = _optionsSnapshot.Get(connectionKey);
        
        if (options is null || options.ConnectionKey != connectionKey)
            throw new InvalidOperationException(string.Format(Strings.InvalidConnectionKeyX, connectionKey));

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