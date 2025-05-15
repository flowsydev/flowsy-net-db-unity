namespace Flowsy.Db.Unity.Conventions;

/// <summary>
/// Base class for building database conventions.
/// </summary>
public abstract class DbConventionBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DbConventionBuilder"/> class.
    /// </summary>
    /// <param name="parent">
    /// The parent builder that this convention builder belongs to.
    /// </param>
    protected DbConventionBuilder(DbConventionSetBuilder parent)
    {
        Parent = parent;
    }

    /// <summary>
    /// Gets the parent builder that this convention builder belongs to.
    /// </summary>
    public DbConventionSetBuilder Parent { get; }
    
    /// <summary>
    /// Allows to configure conventions for database routines. 
    /// </summary>
    /// <returns>
    /// A <see cref="DbRoutineConventionBuilder"/> instance to configure routine conventions.
    /// </returns>
    public DbRoutineConventionBuilder ForRoutines() => Parent.ForRoutines();
    
    /// <summary>
    /// Allows to configure conventions for database parameters.
    /// </summary>
    /// <returns>
    /// A <see cref="DbParameterConventionBuilder"/> instance to configure parameter conventions.
    /// </returns>
    public DbParameterConventionBuilder ForParameters() => Parent.ForParameters();
    
    /// <summary>
    /// Allows to configure conventions for enum types.
    /// </summary>
    /// <returns>
    /// A <see cref="DbEnumConventionBuilder"/> instance to configure enum conventions.
    /// </returns>
    public DbEnumConventionBuilder ForEnums() => Parent.ForEnums();
    
    /// <summary>
    /// Allows to configure conventions for date and time types.
    /// </summary>
    /// <returns>
    /// A <see cref="DbDateTimeConventionBuilder"/> instance to configure date and time conventions.
    /// </returns>
    public DbDateTimeConventionBuilder ForDateTimes() => Parent.ForDateTimes();
    
    /// <summary>
    /// Allows to configure conventions for executing database commands.
    /// </summary>
    /// <returns>
    /// A <see cref="DbCommandConventionBuilder"/> instance to configure command conventions.
    /// </returns>
    public DbCommandConventionBuilder ForCommands() => Parent.ForCommands();
    
    /// <summary>
    /// Builds the final <see cref="DbConventionSet"/> based on the configured conventions.
    /// </summary>
    /// <returns>
    /// The built <see cref="DbConventionSet"/> instance containing all the configured conventions.
    /// </returns>
    public DbConventionSet Build() => Parent.Build();
}