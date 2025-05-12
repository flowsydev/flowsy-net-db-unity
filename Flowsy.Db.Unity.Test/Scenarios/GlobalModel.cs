namespace Flowsy.Db.Unity.Test.Scenarios;

public class GlobalModel : IDisposable
{
    public IDictionary<DbProviderFamily, DbProviderDescriptor> Providers { get; } = new Dictionary<DbProviderFamily, DbProviderDescriptor>();

    public void Dispose()
    {
    }
}