using System.Data;

namespace Flowsy.Db.Unity;

public interface IDbUnitOfWork : IDisposable, IAsyncDisposable
{
    IDbConnection Connection { get; }
    
    IDbTransaction? Transaction { get; }
    
    public event EventHandler? WorkBegun;
    public event EventHandler? WorkCompleted;
    public event EventHandler? WorkDiscarded;

    void BeginWork();

    TService InvolveService<TService>(Func<Type, bool>? implementationSelector = null) where TService : class;

    void CompleteWork();
    Task CompleteWorkAsync(CancellationToken cancellationToken = default);
    
    void DiscardWork();
    Task DiscardWorkAsync(CancellationToken cancellationToken = default);
}