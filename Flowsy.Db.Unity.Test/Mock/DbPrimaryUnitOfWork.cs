using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Flowsy.Db.Unity.Test.Mock;

public interface IDbPrimaryUnitOfWork : IDbUnitOfWork;

public class DbPrimaryUnitOfWork : DbUnitOfWork, IDbPrimaryUnitOfWork
{
    public DbPrimaryUnitOfWork(IOptionsSnapshot<DbConnectionOptions> optionsSnapshot, IDbConnectionScope connectionScope, ILogger<DbPrimaryUnitOfWork> logger)
        : base(optionsSnapshot.Get("Primary"), connectionScope, logger)
    {
    }
}