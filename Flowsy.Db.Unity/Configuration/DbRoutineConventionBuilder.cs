using Flowsy.Core;

namespace Flowsy.Db.Unity.Configuration;

public class DbRoutineConventionBuilder
{
    private readonly DbConventionSetBuilder _conventionSetBuilder;

    internal DbRoutineConventionBuilder(DbConventionSetBuilder conventionSetBuilder)
    {
        _conventionSetBuilder = conventionSetBuilder;
    }

    public DbRoutineConventionBuilder UseType(DbRoutineType routineType)
    {
        _conventionSetBuilder.ConventionSet.Routines.Type = routineType;
        return this;
    }
    
    public DbRoutineConventionBuilder UseProcedurePrefix(string prefix)
    {
        _conventionSetBuilder.ConventionSet.Routines.ProcedurePrefix = prefix;
        return this;
    }
    
    public DbRoutineConventionBuilder UseProcedureSuffix(string suffix)
    {
        _conventionSetBuilder.ConventionSet.Routines.ProcedureSuffix = suffix;
        return this;
    }

    public DbRoutineConventionBuilder UseFunctionPrefix(string prefix)
    {
        _conventionSetBuilder.ConventionSet.Routines.FunctionPrefix = prefix;
        return this;
    }
    
    public DbRoutineConventionBuilder UseFunctionSuffix(string suffix)
    {
        _conventionSetBuilder.ConventionSet.Routines.FunctionSuffx = suffix;
        return this;
    }
    
    public DbRoutineConventionBuilder UseCaseStyle(CaseStyle? caseStyle)
    {
        _conventionSetBuilder.ConventionSet.Routines.CaseStyle = caseStyle;
        return this;
    }

    public DbRoutineConventionBuilder ForRoutines() => _conventionSetBuilder.ForRoutines();
    public DbParameterConventionBuilder ForParameters() => _conventionSetBuilder.ForParameters();
    
    public DbEnumConventionBuilder ForEnums() => _conventionSetBuilder.ForEnums();
    
}