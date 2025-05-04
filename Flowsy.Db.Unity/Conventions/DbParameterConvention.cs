using Flowsy.Core;

namespace Flowsy.Db.Unity.Conventions;

public class DbParameterConvention : DbConvention
{
    public DbParameterConvention(DbConventionSet conventions) : base(conventions)
    {
    }

    public string Prefix { get; internal set; } = string.Empty;
    public string Suffix { get; internal set; } = string.Empty;
    public CaseStyle? CaseStyle { get; internal set; }
    
    public void CopyTo(DbParameterConvention other)
    {
        other.Prefix = Prefix;
        other.Suffix = Suffix;
        other.CaseStyle = CaseStyle;
    }

    public DbParameterConvention Clone()
    {
        var clone = new DbParameterConvention(Conventions);
        CopyTo(clone);
        return clone;
    }
}