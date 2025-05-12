namespace Flowsy.Db.Unity.Conventions;

public class DbJsonConvention : DbConvention
{
    public DbJsonConvention(DbConventionSet conventions) : base(conventions)
    {
    }
    
    public Func<object, string>? Serialize { get; internal set; }
    
    public void CopyTo(DbJsonConvention other)
    {
        other.Serialize = Serialize;
    }
    
    public DbJsonConvention Clone(DbConventionSet parentConventions)
    {
        var clone = new DbJsonConvention(parentConventions);
        CopyTo(clone);
        return clone;
    }
}