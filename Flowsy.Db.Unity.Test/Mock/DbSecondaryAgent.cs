using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Flowsy.Db.Unity.Test.Mock;

public interface IDbSecondaryAgent : IDbAgent;

public class DbSecondaryAgent : DbAgent, IDbSecondaryAgent
{
    public DbSecondaryAgent(IOptionsMonitor<DbConnectionOptions> optionsMonitor, IDbConnectionFactory connectionFactory, ILogger<DbSecondaryAgent> logger)
        : base(optionsMonitor.Get("Secondary"), connectionFactory, logger)
    {
    }
}