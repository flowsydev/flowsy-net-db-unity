using System.Data;

namespace Flowsy.Db.Unity;

public interface IDbUnitOfWork
{
    IDbConnection Connection { get; }
    
    TService InvolveService<TService>() where TService : class;
}