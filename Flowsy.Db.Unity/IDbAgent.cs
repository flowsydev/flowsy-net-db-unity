using System.Data;

namespace Flowsy.Db.Unity;

public interface IDbAgent
{
    IDbConnection Connection { get; }
    IDbUnitOfWork? UnitOfWork { get; }
}