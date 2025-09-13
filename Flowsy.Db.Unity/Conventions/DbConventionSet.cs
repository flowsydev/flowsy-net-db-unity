namespace Flowsy.Db.Unity.Conventions;

/// <summary>
/// Represents a database convention set that defines how to interact with database objects.
/// </summary>
public record DbConventionSet
{
    /// <summary>
    /// Default convention set that uses generic configurations.
    /// </summary>
    public static readonly DbConventionSet Default = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="DbConventionSet"/> class with default values.
    /// </summary>
    private DbConventionSet()
    {
        Provider = DbProviderDescriptor.Generic;
        Routines = DbRoutineConvention.Default with { ConventionSet = this };
        Parameters = DbParameterConvention.Default with { ConventionSet = this };
        Enums = DbEnumConvention.Default with { ConventionSet = this };
        DateTime = DbDateTimeConvention.Default;
        Commands = DbCommandConvention.Default with { ConventionSet = this };
        DefaultCaseStyle = DbCaseStyle.None;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DbConventionSet"/> class with specific configurations.
    /// </summary>
    /// <param name="provider">
    /// Data provider descriptor for which these conventions apply.
    /// </param>
    /// <param name="routines">
    /// Conventions for database routines (stored procedures and functions).
    /// If <c>null</c>, default conventions are used.
    /// </param>
    /// <param name="parameters">
    /// Conventions for database parameters.
    /// If <c>null</c>, default conventions are used.
    /// </param>
    /// <param name="enums">
    /// Conventions for database enum types.
    /// If <c>null</c>, default conventions are used.
    /// </param>
    /// <param name="dateTime">
    /// Conventions for DateTime and DateTimeOffset types.
    /// If <c>null</c>, default conventions are used.
    /// </param>
    /// <param name="commands">
    /// Conventions for database commands.
    /// If <c>null</c>, default conventions are used.
    /// </param>
    /// <param name="defaultCaseStyle">
    /// Default naming style to apply when no specific one is specified.
    /// If <c>null</c>, <see cref="DbCaseStyle.None"/> is used.
    /// </param>
    public DbConventionSet(
        DbProviderDescriptor provider,
        DbRoutineConvention? routines = null,
        DbParameterConvention? parameters = null,
        DbEnumConvention? enums = null,
        DbDateTimeConvention? dateTime = null,
        DbCommandConvention? commands = null,
        DbCaseStyle? defaultCaseStyle = null
        )
    {
        Provider = provider;
        Routines = (routines ?? DbRoutineConvention.Default) with { ConventionSet = this };
        Parameters = (parameters ?? DbParameterConvention.Default) with { ConventionSet = this };
        Enums = (enums ?? DbEnumConvention.Default) with { ConventionSet = this };
        DateTime = (dateTime ?? DbDateTimeConvention.Default) with { ConventionSet = this };
        Commands = (commands ?? DbCommandConvention.Default) with { ConventionSet = this };
        DefaultCaseStyle = defaultCaseStyle ?? DbCaseStyle.None;
    }

    /// <summary>
    /// Gets the database provider descriptor for which these conventions apply.
    /// </summary>
    public DbProviderDescriptor Provider { get; init; }
    
    /// <summary>
    /// Gets the conventions for database routines (stored procedures and functions).
    /// </summary>
    public DbRoutineConvention Routines { get; init; }
    
    /// <summary>
    /// Gets the conventions for database parameters.
    /// </summary>
    public DbParameterConvention Parameters { get; init; }
    
    /// <summary>
    /// Gets the conventions for database enum types.
    /// </summary>
    public DbEnumConvention Enums { get; init; }
    
    /// <summary>
    /// Gets the conventions for DateTime and DateTimeOffset types.
    /// </summary>
    public DbDateTimeConvention DateTime { get; init; }
    
    /// <summary>
    /// Gets the conventions for database commands.
    /// </summary>
    public DbCommandConvention Commands { get; init; }
    
    /// <summary>
    /// Gets the default naming style to apply when no specific one is specified.
    /// </summary>
    public DbCaseStyle DefaultCaseStyle { get; init; }

    /// <summary>
    /// Deconstructs the convention set into its main components for routines and commands.
    /// </summary>
    /// <param name="routines">
    /// The conventions for database routines.
    /// </param>
    /// <param name="commands">
    /// The conventions for database commands.
    /// </param>
    public void Deconstruct(
        out DbRoutineConvention routines,
        out DbCommandConvention commands
        )
    {
        routines = Routines;
        commands = Commands;
    }

    /// <summary>
    /// Deconstructs the convention set into all its components.
    /// </summary>
    /// <param name="provider">
    /// The database provider descriptor.
    /// </param>
    /// <param name="routines">
    /// The conventions for database routines.
    /// </param>
    /// <param name="parameters">
    /// The conventions for database parameters.
    /// </param>
    /// <param name="enums">
    /// The conventions for database enum types.
    /// </param>
    /// <param name="dateTime">
    /// The conventions for DateTime and DateTimeOffset types.
    /// </param>
    /// <param name="commands">
    /// The conventions for database commands.
    /// </param>
    /// <param name="defaultCaseStyle">
    /// The default naming style.
    /// </param>
    public void Deconstruct(
        out DbProviderDescriptor provider,
        out DbRoutineConvention routines,
        out DbParameterConvention parameters,
        out DbEnumConvention enums,
        out DbDateTimeConvention dateTime,
        out DbCommandConvention commands,
        out DbCaseStyle defaultCaseStyle
        )
    {
        provider = Provider;
        routines = Routines;
        parameters = Parameters;
        enums = Enums;
        dateTime = DateTime;
        commands = Commands;
        defaultCaseStyle = DefaultCaseStyle;
    }
}