using System.Data;
using Flowsy.Db.Unity.Resources;
using Microsoft.Extensions.Logging;

namespace Flowsy.Db.Unity;

public class DbAgent : IDbAgent
{
    private readonly IDbConnectionScope? _connectionScope;
    private IDbConnection? _connection;
    private readonly ILogger? _logger;

    public DbAgent(DbConnectionOptions connectionOptions, ILogger? logger = null)
    {
        ConnectionOptions = connectionOptions;
    }
    
    public DbAgent(DbConnectionOptions connectionOptions, IDbConnectionScope connectionScope, ILogger? logger = null)
    {
        _connectionScope = connectionScope;
        ConnectionOptions = connectionOptions;
        _logger = logger;
    }

    public DbAgent(IDbUnitOfWork unitOfWork, ILogger? logger = null)
    {
        if (unitOfWork is not DbUnitOfWork dbUnitOfWork)
            throw new ArgumentException(string.Format(Strings.InvalidUnitOfWorkTypeX, nameof(DbUnitOfWork)), nameof(unitOfWork));
        
        ConnectionOptions = dbUnitOfWork.ConnectionOptions;
        UnitOfWork = unitOfWork;
        _logger = logger;
    }
    
    protected DbConnectionOptions ConnectionOptions { get; }
    
    public IDbConnection Connection => _connection ??= 
        UnitOfWork?.Connection ??
        _connectionScope?.GetConnection(ConnectionOptions.ConnectionString) ??
        ConnectionOptions.CreateConnection() ??
        throw new InvalidOperationException(string.Format(Strings.CouldNotResolveConnectionForKeyX, ConnectionOptions.ConnectionKey));
    
    public IDbUnitOfWork? UnitOfWork { get; }
}