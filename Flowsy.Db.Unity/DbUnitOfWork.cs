using System.Data;
using Flowsy.Db.Unity.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Flowsy.Db.Unity;

public class DbUnitOfWork : IDbUnitOfWork
{
    private readonly IServiceProvider? _provider;
    private readonly IDbConnectionScope? _connectionScope;
    private IDbConnection? _connection;
    private readonly ILogger? _logger;

    public DbUnitOfWork(DbConnectionOptions connectionOptions, ILogger? logger = null)
    {
        ConnectionOptions = connectionOptions;
        _logger = logger;
    }
    
    public DbUnitOfWork(DbConnectionOptions connectionOptions, IServiceProvider provider, ILogger? logger = null)
    {
        ConnectionOptions = connectionOptions;
        _provider = provider;
        _connectionScope = provider.GetService<IDbConnectionScope>();
        _logger = logger;
    }

    protected internal DbConnectionOptions ConnectionOptions { get; }
    public IDbConnection Connection => _connection ??=
        _connectionScope?.GetConnection(ConnectionOptions.ConnectionKey) ??
        throw new InvalidOperationException(string.Format(Strings.CouldNotResolveConnectionForKeyX, ConnectionOptions.ConnectionKey));

    public TService InvolveService<TService>() where TService : class
    {
        throw new NotImplementedException();
    }
}