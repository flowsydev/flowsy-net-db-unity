using System.Data;
using System.Data.Common;
using System.Reflection;
using Flowsy.Db.Unity.Extensions;
using Flowsy.Db.Unity.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Flowsy.Db.Unity;

public class DbUnitOfWork : IDbUnitOfWork
{
    private readonly IServiceProvider? _serviceProvider;
    private readonly IDbConnectionScope? _connectionScope;
    private IDbConnection? _connection;
    private readonly ILogger? _logger;
    private bool _disposed;

    public DbUnitOfWork(DbConnectionOptions connectionOptions, ILogger? logger = null)
    {
        ConnectionOptions = connectionOptions;
        _logger = logger;
    }
    
    public DbUnitOfWork(DbConnectionOptions connectionOptions, IServiceProvider serviceProvider, ILogger? logger = null)
    {
        ConnectionOptions = connectionOptions;
        _serviceProvider = serviceProvider;
        _connectionScope = serviceProvider.GetService<IDbConnectionScope>();
        _logger = logger;
    }
    
    ~DbUnitOfWork()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            TryRollbackTransaction();
            _connection?.Dispose();
        }

        _disposed = true;
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            await TryRollbackTransactionAsync(CancellationToken.None);
            if (_connection is DbConnection dbConnection)
                await dbConnection.DisposeAsync();
            else
                _connection?.Dispose();
        }
        
        _disposed = true;
    }

    protected internal DbConnectionOptions ConnectionOptions { get; }
    
    public IDbConnection Connection => _connection ??=
        _connectionScope?.GetConnection(ConnectionOptions.ConnectionKey) ??
        throw new InvalidOperationException(string.Format(Strings.CouldNotResolveConnectionForKeyX, ConnectionOptions.ConnectionKey));

    public IDbTransaction? Transaction { get; private set; }
    
    public event EventHandler? WorkBegun;
    public event EventHandler? WorkCompleted;
    public event EventHandler? WorkDiscarded;
    
    private void EnsureWorkBegun()
    {
        if (Transaction is null)
            throw new InvalidOperationException(string.Format(Strings.MustBeginWorkInvokingMethodX, nameof(BeginWork)));
    }

    public void BeginWork()
    {
        if (Transaction is not null)
            throw new InvalidOperationException(Strings.TheWorkHasAlreadyBegunForThisUnit);
        
        if (Connection.State != ConnectionState.Open)
            Connection.Open();
        
        Transaction = Connection.BeginTransaction();
        OnWorkBegun();
    }
    
    /// <summary>
    /// Triggers the <see cref="WorkBegun"/> event.
    /// </summary>
    protected virtual void OnWorkBegun()
    {
        if (_logger is not null && _logger.IsEnabled(ConnectionOptions.LogLevel))
        {
            _logger.Log(
                ConnectionOptions.LogLevel,
                "[ {DatabaseUrl} ] Unit of work started",
                Connection.GetDatabaseUrl()
            );
        }

        WorkBegun?.Invoke(this, EventArgs.Empty);
    }

    public TService InvolveService<TService>(Func<Type, bool>? implementationSelector = null) where TService : class
    {
        if (_serviceProvider is null)
            throw new InvalidOperationException(string.Format(Strings.CouldNotResolveServiceProvider));
        
        var serviceType = typeof(TService);
        var serviceDescriptors = _serviceProvider.GetRequiredService<IReadOnlyCollection<ServiceDescriptor>>();
        var serviceDescriptor = serviceDescriptors.FirstOrDefault(sd => sd.ServiceType == serviceType && (implementationSelector is null || implementationSelector(sd.ImplementationType ?? sd.ServiceType)));
        if (serviceDescriptor is null)
            throw new InvalidOperationException(string.Format(Strings.CouldNotFindImplementationForTypeX, serviceType.FullName));
        
        var implementationType = serviceDescriptor.ImplementationType ?? serviceType;
        var implementationTypeConstructors = implementationType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
        
        var implementationTypeConstructor = (
            from c in implementationTypeConstructors
            orderby c.GetParameters().Length descending
            select c
        ).FirstOrDefault();
        
        if (implementationTypeConstructor is null)
            throw new InvalidOperationException(string.Format(Strings.CouldNotFindConstructorForTypeX, implementationType));
        
        var unitOfWorkInterfaceType = typeof(IDbUnitOfWork);
        
        var args = new List<object?>();
        foreach (var parameter in implementationTypeConstructor.GetParameters())
        {
            if (unitOfWorkInterfaceType.IsAssignableFrom(parameter.ParameterType))
            {
                args.Add(this);
                continue;
            }
            
            var service = _serviceProvider.GetService(parameter.ParameterType);
            args.Add(service);
        }
        
        var constructorParameterTypes = implementationTypeConstructor.GetParameters().Select(p => p.ParameterType).ToArray();
        var factory = ActivatorUtilities.CreateFactory(implementationType, constructorParameterTypes);

        return (TService) factory(_serviceProvider, args.ToArray());
    }
    
    public void CompleteWork()
    {
        EnsureWorkBegun();
        
        Transaction!.Commit();
        Transaction.Dispose();
        Transaction = null;
        
        OnWorkCompleted();
    }

    public async Task CompleteWorkAsync(CancellationToken cancellationToken = default)
    {
        EnsureWorkBegun();

        if (Transaction is DbTransaction dbTransaction)
        {
            await dbTransaction.CommitAsync(cancellationToken);
            await dbTransaction.DisposeAsync();
        }
        else
        {
            Transaction!.Commit();
            Transaction.Dispose();
        }
        
        Transaction = null;
        
        OnWorkCompleted();
    }
    
    protected virtual void OnWorkCompleted()
    {
        if (_logger is not null && _logger.IsEnabled(ConnectionOptions.LogLevel))
        {
            _logger.Log(
                ConnectionOptions.LogLevel,
                "[ {DatabaseUrl} ] Unit of work completed",
                Connection.GetDatabaseUrl()
            );
        }
        
        WorkCompleted?.Invoke(this, EventArgs.Empty);
    }

    private void TryRollbackTransaction()
    {
        if (Transaction is null)
            return;
        
        Transaction.Rollback();
        Transaction.Dispose();
        Transaction = null;
    }
    
    public void DiscardWork()
    {
        TryRollbackTransaction();
        OnWorkDiscarded();
    }
    
    private async Task TryRollbackTransactionAsync(CancellationToken cancellationToken)
    {
        switch (Transaction)
        {
            case null:
                return;
            case DbTransaction dbTransaction:
                await dbTransaction.RollbackAsync(cancellationToken);
                await dbTransaction.DisposeAsync();
                break;
            default:
                Transaction.Rollback();
                Transaction.Dispose();
                break;
        }

        Transaction = null;
    }

    public async Task DiscardWorkAsync(CancellationToken cancellationToken = default)
    {
        await TryRollbackTransactionAsync(cancellationToken);
        OnWorkDiscarded();
    }
    
    protected virtual void OnWorkDiscarded()
    {
        if (_logger is not null && _logger.IsEnabled(ConnectionOptions.LogLevel))
        {
            _logger.Log(
                ConnectionOptions.LogLevel,
                "[ {DatabaseUrl} ] Unit of work discarded",
                Connection.GetDatabaseUrl()
            );
        }
        
        WorkDiscarded?.Invoke(this, EventArgs.Empty);
    }
}