namespace Flowsy.Db.Unity.Conventions;

public class DbRoutineNamingConvention : DbConvention
{
    internal DbRoutineNamingConvention(DbConventionSet conventions) : base(conventions)
    {
    }

    public DbObjectNameConvention Naming { get; internal set; } = new();
    public bool UseNamedParameters { get; internal set; }
    
    public void CopyTo(DbRoutineNamingConvention other)
    {
        other.Naming.CaseStyle = Naming.CaseStyle;
        other.Naming.Prefix = Naming.Prefix;
        other.Naming.Suffix = Naming.Suffix;
        other.UseNamedParameters = UseNamedParameters;
    }
    
    public DbRoutineNamingConvention Clone(DbConventionSet conventions)
    {
        var clone = new DbRoutineNamingConvention(conventions);
        CopyTo(clone);
        return clone;
    }
}