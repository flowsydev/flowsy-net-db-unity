using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Flowsy.Db.Unity.Test.Mock;

public interface IDbPrimaryAgent : IDbAgent;

public class DbPrimaryAgent : DbAgent, IDbPrimaryAgent
{
    public DbPrimaryAgent(IOptionsMonitor<DbConnectionOptions> optionsMonitor, IDbConnectionFactory connectionFactory, ILogger<DbPrimaryAgent> logger)
        : base(optionsMonitor.Get("Primary"), connectionFactory, logger)
    {
    }
}