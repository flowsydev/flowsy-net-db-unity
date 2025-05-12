using Flowsy.Core;

namespace Flowsy.Db.Unity.Conventions;

public class DbConventionSetBuilder
{
    private readonly DbRoutineConventionBuilder _routineConventionBuilder;
    private readonly DbParameterConventionBuilder _parameterConventionBuilder;
    private readonly DbEnumConventionBuilder _enumConventionBuilder;
    private readonly DbDateTimeConventionBuilder _dateTimeConventionBuilder;
    private readonly DbCommandConventionBuilder _commandConventionBuilder;

    internal DbConventionSetBuilder(DbProviderDescriptor provider)
    {
        Conventions = DbConventionSet.Default.Clone();
        Conventions.Provider = provider;
        _routineConventionBuilder = new DbRoutineConventionBuilder(this);
        _parameterConventionBuilder = new DbParameterConventionBuilder(this);
        _enumConventionBuilder = new DbEnumConventionBuilder(this);
        _dateTimeConventionBuilder = new DbDateTimeConventionBuilder(this);
        _commandConventionBuilder = new DbCommandConventionBuilder(this);
    }

    internal DbConventionSetBuilder(DbConventionSet conventions)
    {
        Conventions = conventions;
        Conventions.Provider = Conventions.Provider;
        _routineConventionBuilder = new DbRoutineConventionBuilder(this);
        _parameterConventionBuilder = new DbParameterConventionBuilder(this);
        _enumConventionBuilder = new DbEnumConventionBuilder(this);
        _dateTimeConventionBuilder = new DbDateTimeConventionBuilder(this);
        _commandConventionBuilder = new DbCommandConventionBuilder(this);
    }
    
    internal DbConnectionOptionsBuilder? ConnectionOptionsBuilder { get; set; } 
    internal DbConventionSet Conventions { get; }

    public DbConventionSetBuilder UseDefaultCaseStyle(CaseStyle? caseStyle)
    {
        Conventions.DefaultCaseStyle = caseStyle;
        return this;
    }
    
    public DbRoutineConventionBuilder ForRoutines() => _routineConventionBuilder;
    
    public DbParameterConventionBuilder ForParameters() => _parameterConventionBuilder;
    
    public DbEnumConventionBuilder ForEnums() => _enumConventionBuilder;
    public DbDateTimeConventionBuilder ForDateTimes() => _dateTimeConventionBuilder;
    public DbCommandConventionBuilder ForCommands() => _commandConventionBuilder;

    public DbConventionSet Build() => Conventions;
}