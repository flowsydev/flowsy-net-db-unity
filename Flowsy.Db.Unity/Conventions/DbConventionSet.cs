using Flowsy.Core;

namespace Flowsy.Db.Unity.Conventions;

/// <summary>
/// Represents a set of conventions for database operations.
/// A set of conventions defines naming preferences and other options to be used when executing database queries and commands.
/// </summary>
public class DbConventionSet
{
    /// <summary>
    /// The default convention set that is used when no specific conventions are provided.
    /// </summary>
    public static DbConventionSet Default { get; set; } = new (DbProviderDescriptor.Generic);

    /// <summary>
    /// Initializes a new instance of the <see cref="DbConventionSet"/> class.
    /// </summary>
    /// <param name="provider">
    /// The database provider descriptor for which the conventions are defined.
    /// </param>
    internal DbConventionSet(DbProviderDescriptor provider)
    {
        Provider = provider;
        if (ReferenceEquals(Default, null))
        {
            Routines = new DbRoutineConvention(this);
            Parameters = new DbParameterConvention(this);
            Enums = new DbEnumConvention(this);
            DateTime = new DbDateTimeConvention(this);
            Commands = new DbCommandConvention(this);
        }
        else
        {
            Routines = Default.Routines.Clone(this);
            Parameters = Default.Parameters.Clone(this);
            Enums = Default.Enums.Clone(this);
            DateTime = Default.DateTime.Clone(this);
            Commands = Default.Commands.Clone(this);
        }
    }

    internal DbConventionSet(
        DbProviderDescriptor provider,
        DbRoutineConvention routines,
        DbParameterConvention parameters,
        DbEnumConvention enums,
        DbDateTimeConvention dateTime,
        DbCommandConvention commands
        )
    {
        Provider = provider;
        Routines = routines;
        Parameters = parameters;
        Enums = enums;
        DateTime = dateTime;
        Commands = commands;
    }

    /// <summary>
    /// Represents the database provider for which the conventions are defined.
    /// </summary>
    public DbProviderDescriptor Provider { get; internal set; }
    
    /// <summary>
    /// The default case style to be used for database object names.
    /// </summary>
    public CaseStyle? DefaultCaseStyle { get; internal set; }
    
    /// <summary>
    /// The conventions for database routines, including stored procedures and functions.
    /// </summary>
    public DbRoutineConvention Routines { get; internal set; }
    
    /// <summary>
    /// The conventions for database parameters.
    /// </summary>
    public DbParameterConvention Parameters { get; internal set; }
    
    /// <summary>
    /// The conventions for enum types.
    /// </summary>
    public DbEnumConvention Enums { get; internal set; }
    
    /// <summary>
    /// The conventions for date and time types.
    /// </summary>
    public DbDateTimeConvention DateTime { get; internal set; }
    
    /// <summary>
    /// The conventions for executing database commands.
    /// </summary>
    public DbCommandConvention Commands { get; internal set; }

    /// <summary>
    /// Copies the conventions from this instance to another instance of <see cref="DbConventionSet"/>.
    /// </summary>
    /// <param name="other"></param>
    public void CopyTo(DbConventionSet other)
    {
        other.Provider = Provider;
        other.DefaultCaseStyle = DefaultCaseStyle;
        Routines.CopyTo(other.Routines);
        Parameters.CopyTo(other.Parameters);
        Enums.CopyTo(other.Enums);
        DateTime.CopyTo(other.DateTime);
        Commands.CopyTo(other.Commands);
    }
    
    /// <summary>
    /// Creates a clone of the current <see cref="DbConventionSet"/> instance.
    /// </summary>
    /// <returns></returns>
    public DbConventionSet Clone()
    {
        var clone = new DbConventionSet(Provider);
        CopyTo(clone);
        return clone;
    }
    
    /// <summary>
    /// Creates a new <see cref="DbConventionSetBuilder"/> instance for configuring a set of conventions.
    /// </summary>
    /// <param name="provider">
    /// The database provider descriptor for which the conventions are defined.
    /// </param>
    /// <returns>
    /// A <see cref="DbConventionSetBuilder"/> instance for configuring the conventions.
    /// </returns>
    public static DbConventionSetBuilder CreateBuilder(DbProviderDescriptor provider) => new (provider);
    
    /// <summary>
    /// Creates a new <see cref="DbConventionSetBuilder"/> instance for configuring a set of conventions based on an existing convention set.
    /// </summary>
    /// <param name="conventions">
    /// The existing <see cref="DbConventionSet"/> instance to be used as a base for the new builder.
    /// </param>
    /// <returns>
    /// A <see cref="DbConventionSetBuilder"/> instance for configuring the conventions.
    /// </returns>
    public static DbConventionSetBuilder CreateBuilder(DbConventionSet conventions) => new (conventions);
}