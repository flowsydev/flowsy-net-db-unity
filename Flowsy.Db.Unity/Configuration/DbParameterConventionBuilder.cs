using Flowsy.Core;
using Flowsy.Db.Unity.Conventions;

namespace Flowsy.Db.Unity.Configuration;

public class DbParameterConventionBuilder
{
    private readonly DbConventionSetBuilder _conventionSetBuilder;
    
    internal DbParameterConventionBuilder(DbConventionSetBuilder conventionSetBuilder)
    {
        _conventionSetBuilder = conventionSetBuilder;
    }

    public DbParameterConventionBuilder UsePrefix(string prefix)
    {
        _conventionSetBuilder.ConventionSet.Parameters.Prefix = prefix;
        return this;
    }
    
    public DbParameterConventionBuilder UseSuffix(string suffix)
    {
        _conventionSetBuilder.ConventionSet.Parameters.Suffix = suffix;
        return this;
    }
    
    public DbParameterConventionBuilder UseCaseStyle(CaseStyle? caseStyle)
    {
        _conventionSetBuilder.ConventionSet.Parameters.CaseStyle = caseStyle;
        return this;
    }

    public DbRoutineConventionBuilder ForRoutines() => _conventionSetBuilder.ForRoutines();
    
    public DbParameterConventionBuilder ForParameters() => _conventionSetBuilder.ForParameters();
    
    public DbEnumConventionBuilder ForEnums() => _conventionSetBuilder.ForEnums();
}