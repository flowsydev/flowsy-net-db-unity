using Flowsy.Core;

namespace Flowsy.Db.Unity.Conventions;

public class DbRoutineConvention : DbConvention
{
    public DbRoutineConvention(DbConventionSet conventions) : base(conventions)
    {
    }

    public DbRoutineType Type { get; internal set; } = DbRoutineType.Procedure;
    
    public string ProcedurePrefix { get; internal set; } = string.Empty;
    public string ProcedureSuffix { get; internal set; } = string.Empty;
    
    public string FunctionPrefix { get; internal set; } = string.Empty;
    
    public string FunctionSuffx { get; internal set; } = string.Empty;
    
    public CaseStyle? CaseStyle { get; internal set; }

    public void CopyTo(DbRoutineConvention other)
    {
        other.Type = Type;
        other.ProcedurePrefix = ProcedurePrefix;
        other.ProcedureSuffix = ProcedureSuffix;
        other.FunctionPrefix = FunctionPrefix;
        other.FunctionSuffx = FunctionSuffx;
        other.CaseStyle = CaseStyle;
    }

    public DbRoutineConvention Clone()
    {
        var clone = new DbRoutineConvention(Conventions);
        CopyTo(clone);
        return clone;
    }
}