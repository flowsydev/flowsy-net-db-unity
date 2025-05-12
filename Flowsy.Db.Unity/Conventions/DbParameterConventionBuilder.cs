using Flowsy.Core;

namespace Flowsy.Db.Unity.Conventions;

public class DbParameterConventionBuilder : DbConventionBuilder
{
    internal DbParameterConventionBuilder(DbConventionSetBuilder parent) : base(parent)
    {
    }
    
    public DbParameterConventionBuilder UseNames(CaseStyle? caseStyle, string? prefix = null, string? suffix = null)
    {
        Parent.Conventions.Parameters.Naming.CaseStyle = caseStyle;
        Parent.Conventions.Parameters.Naming.Prefix = prefix ?? string.Empty;
        Parent.Conventions.Parameters.Naming.Suffix = suffix ?? string.Empty;
        return this;
    }
    
    public DbParameterConventionBuilder UseCaseStyle(CaseStyle? caseStyle)
    {
        Parent.Conventions.Parameters.Naming.CaseStyle = caseStyle;
        return this;
    }

    public DbParameterConventionBuilder UsePrefix(string prefix)
    {
        Parent.Conventions.Parameters.Naming.Prefix = prefix;
        return this;
    }
    
    public DbParameterConventionBuilder UseSuffix(string suffix)
    {
        Parent.Conventions.Parameters.Naming.Suffix = suffix;
        return this;
    }
}