using Flowsy.Core;

namespace Flowsy.Db.Unity.Conventions;

public class DbRoutineConventionBuilder : DbConventionBuilder
{
    internal DbRoutineConventionBuilder(DbConventionSetBuilder parent) : base(parent)
    {
    }
    
    public DbRoutineConventionBuilder UseProcedures(CaseStyle? caseStyle = null, string? prefix = null, string? suffix = null, bool useNamedParameters = false)
    {
        Parent.Conventions.Routines.PreferredType = DbRoutineType.StoredProcedure;
        return UseProcedureNames(caseStyle, prefix, suffix, useNamedParameters);
    }
    
    public DbRoutineConventionBuilder UseProcedureNames(CaseStyle? caseStyle = null, string? prefix = null, string? suffix = null, bool useNamedParameters = false)
    {
        Parent.Conventions.Routines.Procedures.Naming.CaseStyle = caseStyle;
        Parent.Conventions.Routines.Procedures.Naming.Prefix = !string.IsNullOrWhiteSpace(prefix) ? prefix : string.Empty;
        Parent.Conventions.Routines.Procedures.Naming.Suffix = !string.IsNullOrWhiteSpace(suffix) ? suffix : string.Empty;
        Parent.Conventions.Routines.Procedures.UseNamedParameters = useNamedParameters;
        return this;
    }

    public DbRoutineConventionBuilder UseFunctions(CaseStyle? caseStyle = null, string? prefix = null, string? suffix = null, bool useNamedParameters = false)
    {
        Parent.Conventions.Routines.PreferredType = DbRoutineType.StoredFunction;
        return UseFunctionNames(caseStyle, prefix, suffix, useNamedParameters);
    }
    
    public DbRoutineConventionBuilder UseFunctionNames(CaseStyle? caseStyle = null, string? prefix = null, string? suffix = null, bool useNamedParameters = false)
    {
        Parent.Conventions.Routines.Functions.Naming.CaseStyle = caseStyle;
        Parent.Conventions.Routines.Functions.Naming.Prefix = !string.IsNullOrWhiteSpace(prefix) ? prefix : string.Empty;
        Parent.Conventions.Routines.Functions.Naming.Suffix = !string.IsNullOrWhiteSpace(suffix) ? suffix : string.Empty;
        Parent.Conventions.Routines.Functions.UseNamedParameters = useNamedParameters;
        return this;
    }
    
    public DbRoutineConventionBuilder UseDefaultType(DbRoutineType preferredType)
    {
        Parent.Conventions.Routines.PreferredType = preferredType;
        return this;
    }
}