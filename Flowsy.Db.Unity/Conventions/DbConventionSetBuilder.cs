using Flowsy.Core;

namespace Flowsy.Db.Unity.Conventions;

/// <summary>
/// A builder for creating a set of conventions for database operations.
/// </summary>
public class DbConventionSetBuilder
{
    private readonly DbRoutineConventionBuilder _routineConventionBuilder;
    private readonly DbParameterConventionBuilder _parameterConventionBuilder;
    private readonly DbEnumConventionBuilder _enumConventionBuilder;
    private readonly DbDateTimeConventionBuilder _dateTimeConventionBuilder;
    private readonly DbCommandConventionBuilder _commandConventionBuilder;

    /// <summary>
    /// Initializes a new instance of the <see cref="DbConventionSetBuilder"/> class.
    /// </summary>
    /// <param name="provider">
    /// The database provider descriptor to be used for the conventions.
    /// </param>
    internal DbConventionSetBuilder(DbProviderDescriptor provider) : this(DbConventionSet.Default.Clone())
    {
        Conventions.Provider = provider;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DbConventionSetBuilder"/> class with an existing set of conventions.
    /// </summary>
    /// <param name="conventions">
    /// The existing <see cref="DbConventionSet"/> to be used as a base for the builder.
    /// </param>
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
    
    /// <summary>
    /// The set of conventions being built.
    /// </summary>
    internal DbConventionSet Conventions { get; }

    /// <summary>
    /// Sets the default case style for database object names.
    /// </summary>
    /// <param name="caseStyle">
    /// The case style to use for database object names. If null, no transformation will be applied when resolving names.
    /// </param>
    /// <returns>
    /// The current instance of <see cref="DbConventionSetBuilder"/> for method chaining.
    /// </returns>
    public DbConventionSetBuilder UseDefaultCaseStyle(CaseStyle? caseStyle)
    {
        Conventions.DefaultCaseStyle = caseStyle;
        return this;
    }
    
    /// <summary>
    /// Allows to configure conventions for database routines.
    /// </summary>
    /// <returns>
    /// A <see cref="DbRoutineConventionBuilder"/> instance to configure routine conventions.
    /// </returns>
    public DbRoutineConventionBuilder ForRoutines() => _routineConventionBuilder;
    
    /// <summary>
    /// Allows to configure conventions for database parameters.
    /// </summary>
    /// <returns>
    /// A <see cref="DbParameterConventionBuilder"/> instance to configure parameter conventions.
    /// </returns>
    public DbParameterConventionBuilder ForParameters() => _parameterConventionBuilder;
    
    /// <summary>
    /// Allows to configure conventions for enum types.
    /// </summary>
    /// <returns>
    /// A <see cref="DbEnumConventionBuilder"/> instance to configure enum conventions.
    /// </returns>
    public DbEnumConventionBuilder ForEnums() => _enumConventionBuilder;
    
    /// <summary>
    /// Allows to configure conventions for date and time types.
    /// </summary>
    /// <returns>
    /// A <see cref="DbDateTimeConventionBuilder"/> instance to configure date and time conventions.
    /// </returns>
    public DbDateTimeConventionBuilder ForDateTimes() => _dateTimeConventionBuilder;
    
    /// <summary>
    /// Allows to configure conventions for executing database commands.
    /// </summary>
    /// <returns>
    /// A <see cref="DbCommandConventionBuilder"/> instance to configure command conventions.
    /// </returns>
    public DbCommandConventionBuilder ForCommands() => _commandConventionBuilder;

    /// <summary>
    /// Builds the final <see cref="DbConventionSet"/> based on the configured conventions.
    /// </summary>
    /// <returns>
    /// The built <see cref="DbConventionSet"/> instance containing all the configured conventions.
    /// </returns>
    public DbConventionSet Build() => Conventions;
}