using Flowsy.Db.Unity.Conventions;

namespace Flowsy.Db.Unity.Configuration;

public class DbConventionSetBuilder
{
    private readonly DbRoutineConventionBuilder _routineConventionBuilder;
    private readonly DbParameterConventionBuilder _parameterConventionBuilder;
    private readonly DbEnumConventionBuilder _enumConventionBuilder;
    
    internal DbConventionSetBuilder(DbProvider provider)
    {
        ConventionSet = DbConventionSet.Default.Clone();
        ConventionSet.Provider = provider;
        
        _routineConventionBuilder = new DbRoutineConventionBuilder(this);
        _parameterConventionBuilder = new DbParameterConventionBuilder(this);
        _enumConventionBuilder = new DbEnumConventionBuilder(this);
    }
    
    internal DbConventionSet ConventionSet { get; }
    
    public DbRoutineConventionBuilder ForRoutines() => _routineConventionBuilder;
    
    public DbParameterConventionBuilder ForParameters() => _parameterConventionBuilder;
    
    public DbEnumConventionBuilder ForEnums() => _enumConventionBuilder;

    public DbConventionSet Build() => ConventionSet;
}