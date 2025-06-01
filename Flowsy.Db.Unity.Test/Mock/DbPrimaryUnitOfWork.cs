using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Flowsy.Db.Unity.Test.Mock;

public interface IDbPrimaryUnitOfWork : IDbUnitOfWork;

public class DbPrimaryUnitOfWork : DbUnitOfWork, IDbPrimaryUnitOfWork
{
    public DbPrimaryUnitOfWork(IOptionsSnapshot<DbConnectionOptions> optionsSnapshot, IDbConnectionFactory connectionFactory, ILogger<DbPrimaryUnitOfWork> logger)
        : base(optionsSnapshot.Get("Primary"), connectionFactory, logger)
    {
    }
}