using Flowsy.Db.Unity.Configuration;

namespace Flowsy.Db.Unity.Conventions;

public class DbConventionSet
{
    public static DbConventionSet Default { get; set; } = new (DbProvider.Generic);

    public DbConventionSet(DbProvider provider)
    {
        Provider = provider;
        if (ReferenceEquals(Default, null))
        {
            Routines = new DbRoutineConvention(this);
            Parameters = new DbParameterConvention(this);
            Enums = new DbEnumConvention(this);
        }
        else
        {
            Routines = Default.Routines.Clone();
            Parameters = Default.Parameters.Clone();
            Enums = Default.Enums.Clone();   
        }
    }

    public DbConventionSet(DbProvider provider, DbRoutineConvention routines, DbParameterConvention parameters, DbEnumConvention enums)
    {
        Provider = provider;
        Routines = routines;
        Parameters = parameters;
        Enums = enums;
    }

    public DbProvider Provider { get; set; }
    public DbRoutineConvention Routines { get; internal set; }
    
    public DbParameterConvention Parameters { get; internal set; }
    
    public DbEnumConvention Enums { get; internal set; }

    public void CopyTo(DbConventionSet other)
    {
        other.Provider = Provider;
        Routines.CopyTo(other.Routines);
        Parameters.CopyTo(other.Parameters);
        Enums.CopyTo(other.Enums);
    }
    
    public DbConventionSet Clone()
    {
        var clone = new DbConventionSet(Provider);
        CopyTo(clone);
        return clone;
    }
    
    public static DbConventionSetBuilder CreateBuilder(DbProvider provider) => new (provider);
}