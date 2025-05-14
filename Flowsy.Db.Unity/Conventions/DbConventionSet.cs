using Flowsy.Core;

namespace Flowsy.Db.Unity.Conventions;

/// <summary>
/// Represents a set of conventions for database operations.
/// A set of conventions defines naming preferences and other options to be used when executing database queries and commands.
/// </summary>
public class DbConventionSet
{
    public static DbConventionSet Default { get; set; } = new (DbProviderDescriptor.Generic);

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
        DbJsonConvention json,
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

    public DbProviderDescriptor Provider { get; set; }
    public CaseStyle? DefaultCaseStyle { get; internal set; }
    public DbRoutineConvention Routines { get; internal set; }
    public DbParameterConvention Parameters { get; internal set; }
    public DbEnumConvention Enums { get; internal set; }
    public DbDateTimeConvention DateTime { get; internal set; }
    public DbCommandConvention Commands { get; internal set; }

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
    
    public DbConventionSet Clone()
    {
        var clone = new DbConventionSet(Provider);
        CopyTo(clone);
        return clone;
    }
    
    public static DbConventionSetBuilder CreateBuilder(DbProviderDescriptor provider) => new (provider);
    public static DbConventionSetBuilder CreateBuilder(DbConventionSet conventions) => new (conventions);
}