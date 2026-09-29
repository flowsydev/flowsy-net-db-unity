using Dapper;

namespace Flowsy.Db.Unity.Conventions;

/// <summary>
/// Provides a fluent way to build a custom database conventions set.
/// </summary>
public class DbConventionSetBuilder
{
    private readonly DbProviderDescriptor _provider;
    private DbRoutineConvention? _routines;
    private DbParameterConvention? _parameters;
    private DbEnumConvention? _enums;
    private DbDateTimeConvention? _dateTime;
    private DbCommandConvention? _commands;
    private DbCaseStyle? _defaultCaseStyle;

    /// <summary>
    /// Initializes a new instance of the <see cref="DbConventionSetBuilder"/> class.
    /// </summary>
    /// <param name="provider">
    /// The database provider descriptor for which conventions will be built.
    /// </param>
    public DbConventionSetBuilder(DbProviderDescriptor provider)
    {
        _provider = provider;
    }
    
    /// <summary>
    /// Configures conventions for database routines (stored procedures and functions).
    /// </summary>
    /// <param name="defaultRoutineType">
    /// Default routine type to use when none is explicitly specified.
    /// </param>
    /// <param name="caseStyle">
    /// Naming style to apply to routine names.
    /// If <c>null</c>, the default style of the convention set will be used.
    /// </param>
    /// <param name="prefix">
    /// Optional prefix to add to routine names.
    /// </param>
    /// <param name="suffix">
    /// Optional suffix to add to routine names.
    /// </param>
    /// <returns>
    /// The current builder instance to allow fluent configuration.
    /// </returns>
    public DbConventionSetBuilder ForRoutines(
        DbRoutineType defaultRoutineType,
        DbCaseStyle? caseStyle = null,
        string? prefix = null,
        string? suffix = null
        ) => ForRoutines(new DbRoutineConvention(defaultRoutineType, caseStyle, prefix, suffix));

    /// <summary>
    /// Configures conventions for database routines using a specific convention.
    /// </summary>
    /// <param name="convention">
    /// The routine convention to use.
    /// </param>
    /// <returns>
    /// The current builder instance to allow fluent configuration.
    /// </returns>
    public DbConventionSetBuilder ForRoutines(DbRoutineConvention convention)
    {
        _routines = convention;
        return this;
    }
    
    /// <summary>
    /// Configures conventions for database parameters.
    /// </summary>
    /// <param name="caseStyle">
    /// Naming style to apply to parameter names.
    /// If <c>null</c>, the default style of the convention set will be used.
    /// </param>
    /// <param name="prefix">
    /// Optional prefix to add to parameter names.
    /// </param>
    /// <param name="suffix">
    /// Optional suffix to add to parameter names.
    /// </param>
    /// <param name="useNamedParameters">
    /// Indicates whether to use named parameters in queries.
    /// </param>
    /// <returns>
    /// The current builder instance to allow fluent configuration.
    /// </returns>
    public DbConventionSetBuilder ForParameters(
        DbCaseStyle? caseStyle = null,
        string? prefix = null,
        string? suffix = null,
        bool useNamedParameters = false
        ) => ForParameters(new DbParameterConvention(caseStyle, prefix, suffix, useNamedParameters));

    /// <summary>
    /// Configures conventions for database parameters using a specific convention.
    /// </summary>
    /// <param name="convention">
    /// The parameter convention to use.
    /// </param>
    /// <returns>
    /// The current builder instance to allow fluent configuration.
    /// </returns>
    public DbConventionSetBuilder ForParameters(DbParameterConvention convention)
    {
        _parameters = convention;
        return this;
    }

    /// <summary>
    /// Configures default parameter conventions and explicit property mappings.
    /// </summary>
    /// <param name="configure">The parameter-convention builder callback.</param>
    /// <returns>The current builder instance.</returns>
    public DbConventionSetBuilder ForParameters(Action<DbParameterConventionBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var builder = new DbParameterConventionBuilder();
        configure(builder);
        return ForParameters(builder.Build());
    }

    /// <summary>
    /// Configures conventions for database enum types.
    /// </summary>
    /// <param name="valueFormat">
    /// Value format for enums to use in the database.
    /// </param>
    /// <param name="memberNameCaseStyle">
    /// Naming style to apply to enum member names.
    /// </param>
    /// <param name="typeNameCaseStyle">
    /// Naming style to apply to enum type names.
    /// </param>
    /// <param name="mappings">
    /// Specific mappings for enum types.
    /// </param>
    /// <returns>
    /// The current builder instance to allow fluent configuration.
    /// </returns>
    public DbConventionSetBuilder ForEnums(DbEnumValueFormat valueFormat = DbEnumValueFormat.Name, DbCaseStyle? memberNameCaseStyle = null, DbCaseStyle? typeNameCaseStyle = null, params DbEnumMapping[] mappings)
        => ForEnums(new DbEnumConvention(valueFormat, new DbEnumNameTranslator(memberNameCaseStyle, typeNameCaseStyle), mappings.ToArray()));
    
    /// <summary>
    /// Configures conventions for database enum types with a custom name translator.
    /// </summary>
    /// <param name="valueFormat">
    /// Value format for enums to use in the database.
    /// </param>
    /// <param name="nameTranslator">
    /// Custom name translator for enum types.
    /// If <c>null</c>, the default translator will be used.
    /// </param>
    /// <param name="mappings">
    /// Specific mappings for enum types.
    /// </param>
    /// <returns>
    /// The current builder instance to allow fluent configuration.
    /// </returns>
    public DbConventionSetBuilder ForEnums(DbEnumValueFormat valueFormat = DbEnumValueFormat.Name, DbEnumNameTranslator? nameTranslator = null, params DbEnumMapping[] mappings)
        => ForEnums(new DbEnumConvention(valueFormat, nameTranslator, mappings.ToArray()));

    /// <summary>
    /// Configures conventions for database enum types using a specific convention.
    /// </summary>
    /// <param name="convention">
    /// The enum type convention to use.
    /// </param>
    /// <returns>
    /// The current builder instance to allow fluent configuration.
    /// </returns>
    public DbConventionSetBuilder ForEnums(DbEnumConvention convention)
    {
        _enums = convention;
        return this;
    }

    /// <summary>
    /// Configures conventions for DateTime and DateTimeOffset types.
    /// </summary>
    /// <param name="offsetValueFormat">
    /// Format to use for DateTimeOffset values in the database.
    /// </param>
    /// <returns>
    /// The current builder instance to allow fluent configuration.
    /// </returns>
    public DbConventionSetBuilder ForDateTime(DbDateTimeOffsetFormat offsetValueFormat)
        => ForDateTime(new DbDateTimeConvention(offsetValueFormat));

    /// <summary>
    /// Configures conventions for DateTime and DateTimeOffset types using a specific convention.
    /// </summary>
    /// <param name="convention">
    /// The DateTime convention to use.
    /// </param>
    /// <returns>
    /// The current builder instance to allow fluent configuration.
    /// </returns>
    public DbConventionSetBuilder ForDateTime(DbDateTimeConvention convention)
    {
        _dateTime = convention;
        return this;
    }

    /// <summary>
    /// Configures conventions for database commands.
    /// </summary>
    /// <param name="timeout">
    /// Optional timeout for database commands in seconds.
    /// If <c>null</c>, the default provider timeout will be used.
    /// </param>
    /// <param name="flags">
    /// Command flags to apply to database commands.
    /// </param>
    /// <returns>
    /// The current builder instance to allow fluent configuration.
    /// </returns>
    public DbConventionSetBuilder ForCommands(int? timeout = null, CommandFlags flags = CommandFlags.Buffered)
        => ForCommands(new DbCommandConvention(timeout, flags));
    
    /// <summary>
    /// Configures conventions for database commands using a specific convention.
    /// </summary>
    /// <param name="convention">
    /// The command convention to use.
    /// </param>
    /// <returns>
    /// The current builder instance to allow fluent configuration.
    /// </returns>
    public DbConventionSetBuilder ForCommands(DbCommandConvention convention)
    {
        _commands = convention;
        return this;
    }
    
    /// <summary>
    /// Sets the default naming style for the convention set.
    /// </summary>
    /// <param name="defaultCaseStyle">
    /// The naming style to use by default.
    /// </param>
    /// <returns>
    /// The current builder instance to allow fluent configuration.
    /// </returns>
    public DbConventionSetBuilder WithDefault(DbCaseStyle defaultCaseStyle)
    {
        _defaultCaseStyle = defaultCaseStyle;
        return this;
    }

    /// <summary>
    /// Builds a database conventions set using the current configurations.
    /// </summary>
    /// <returns>
    /// A new <see cref="DbConventionSet"/> configured with the specified conventions.
    /// </returns>
    public DbConventionSet Build() => new (
        _provider,
        _routines,
        _parameters,
        _enums,
        _dateTime,
        _commands,
        _defaultCaseStyle
        );
}
