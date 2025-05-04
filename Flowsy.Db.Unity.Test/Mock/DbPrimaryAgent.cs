using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Flowsy.Db.Unity.Test.Mock;

public interface IDbPrimaryAgent : IDbAgent;

public class DbPrimaryAgent : DbAgent, IDbPrimaryAgent
{
    public DbPrimaryAgent(IOptionsSnapshot<DbConnectionOptions> optionsSnapshot, ILogger<DbPrimaryAgent> logger)
        : base(optionsSnapshot.Get("Primary"), logger)
    {
    }
}