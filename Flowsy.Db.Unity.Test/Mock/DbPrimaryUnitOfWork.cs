using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Flowsy.Db.Unity.Test.Mock;

public interface IDbPrimaryUnitOfWork : IDbUnitOfWork;

public class DbPrimaryUnitOfWork : DbUnitOfWork, IDbPrimaryUnitOfWork
{
    public DbPrimaryUnitOfWork(
        IOptionsSnapshot<DbConnectionOptions> optionsSnapshot,
        IServiceProvider serviceProvider,
        ILogger<DbPrimaryUnitOfWork> logger
        )
        : base(optionsSnapshot.Get("Primary"), serviceProvider, logger)
    {
    }
}