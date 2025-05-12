using System.Data;

namespace Flowsy.Db.Unity;

public partial interface IDbAgent : IDisposable, IAsyncDisposable
{
    IDbConnection Connection { get; }
    IDbUnitOfWork? UnitOfWork { get; }
    
    event DbCommandExecutingEventHandler? CommandExecuting; 
    event DbCommandExecutedEventHandler? CommandExecuted;
}