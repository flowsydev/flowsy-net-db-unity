namespace Flowsy.Db.Unity.Conventions;

public class DbConventionTypeMapGroup
{
    public IList<Type> Types { get; set; } = [];
    public DbObjectNameConvention ColumnNaming { get; set; } = new();
}