using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Flowsy.Db.Unity.Test.Mock;

public interface IDbSecondaryUnitOfWork : IDbUnitOfWork;

public class DbSecondaryUnitOfWork : DbUnitOfWork, IDbSecondaryUnitOfWork
{
    public DbSecondaryUnitOfWork(IOptionsSnapshot<DbConnectionOptions> optionsSnapshot, IDbConnectionFactory connectionFactory, ILogger<DbSecondaryUnitOfWork> logger)
        : base(optionsSnapshot.Get("Secondary"), connectionFactory, logger)
    {
    }
}