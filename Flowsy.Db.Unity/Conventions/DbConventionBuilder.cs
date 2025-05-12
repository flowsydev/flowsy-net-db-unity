namespace Flowsy.Db.Unity.Conventions;

public abstract class DbConventionBuilder
{
    protected DbConventionBuilder(DbConventionSetBuilder parent)
    {
        Parent = parent;
    }

    public DbConventionSetBuilder Parent { get; }
    
    public DbRoutineConventionBuilder ForRoutines() => Parent.ForRoutines();
    public DbParameterConventionBuilder ForParameters() => Parent.ForParameters();
    public DbEnumConventionBuilder ForEnums() => Parent.ForEnums();
    public DbDateTimeConventionBuilder ForDateTimes() => Parent.ForDateTimes();
    public DbCommandConventionBuilder ForCommands() => Parent.ForCommands();
    public DbConventionSet Build() => Parent.Build();
}