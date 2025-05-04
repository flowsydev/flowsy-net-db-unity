namespace Flowsy.Db.Unity.Conventions;

public abstract class DbConvention
{
    protected DbConvention(DbConventionSet conventions)
    {
        Conventions = conventions;
    }

    public DbConventionSet Conventions { get; }
}