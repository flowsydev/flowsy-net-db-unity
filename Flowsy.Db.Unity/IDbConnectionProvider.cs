using System.Data;

namespace Flowsy.Db.Unity;

/// <summary>
/// Creates connections for configurations recognized by a provider extension.
/// </summary>
public interface IDbConnectionProvider
{
    /// <summary>Indicates whether the provider can handle the specified configuration.</summary>
    bool CanHandle(DbConnectionConfiguration configuration);

    /// <summary>Creates a connection owned by the caller.</summary>
    IDbConnection CreateConnection(DbConnectionConfiguration configuration);
}
