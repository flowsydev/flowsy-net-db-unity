using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Flowsy.Db.Unity.Test.Mock;

public interface IDbSecondaryAgent : IDbAgent;

public class DbSecondaryAgent : DbAgent, IDbSecondaryAgent
{
    public DbSecondaryAgent(IOptionsSnapshot<DbConnectionOptions> optionsSnapshot, ILogger<DbSecondaryAgent> logger)
        : base(optionsSnapshot.Get("Secondary"), logger)
    {
    }
}