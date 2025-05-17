namespace Flowsy.Db.Unity.Conventions;

/// <summary>
/// Conventions for database routines.
/// </summary>
public class DbRoutineNamingConvention : DbConvention
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DbRoutineNamingConvention"/> class.
    /// </summary>
    /// <param name="conventions">
    /// The <see cref="DbConventionSet"/> to which this instance belongs.
    /// </param>
    internal DbRoutineNamingConvention(DbConventionSet conventions) : base(conventions)
    {
    }

    /// <summary>
    /// The naming convention for database routines.
    /// </summary>
    public DbObjectNameConvention Naming { get; internal set; } = new();
    
    /// <summary>
    /// Whether to use named parameters when invoking stored procedures.
    /// Named parameters will be used only if supported by the underlying database provider.
    /// </summary>
    public bool UseNamedParameters { get; internal set; }
    
    /// <summary>
    /// Copies the properties of this instance to another instance of <see cref="DbRoutineNamingConvention"/>.
    /// </summary>
    /// <param name="other">
    /// The <see cref="DbRoutineNamingConvention"/> instance to copy properties to.
    /// </param>
    public void CopyTo(DbRoutineNamingConvention other)
    {
        other.Naming.CaseStyle = Naming.CaseStyle;
        other.Naming.Prefix = Naming.Prefix;
        other.Naming.Suffix = Naming.Suffix;
        other.UseNamedParameters = UseNamedParameters;
    }
    
    /// <summary>
    /// Creates a clone of this <see cref="DbRoutineNamingConvention"/> instance.
    /// </summary>
    /// <param name="conventions">
    /// The <see cref="DbConventionSet"/> to which the cloned instance will belong.
    /// </param>
    /// <returns>
    /// A new instance of <see cref="DbRoutineNamingConvention"/> with the same properties as this instance.
    /// </returns>
    public DbRoutineNamingConvention Clone(DbConventionSet conventions)
    {
        var clone = new DbRoutineNamingConvention(conventions);
        CopyTo(clone);
        return clone;
    }
}