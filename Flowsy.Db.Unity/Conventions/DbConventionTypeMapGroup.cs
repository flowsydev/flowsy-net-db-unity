namespace Flowsy.Db.Unity.Conventions;

/// <summary>
/// Represents a group of types and their associated naming conventions for database column mapping.
/// </summary>
public class DbConventionTypeMapGroup
{
    /// <summary>
    /// A collection of types that belong to this group.
    /// </summary>
    public IList<Type> Types { get; set; } = [];
    
    /// <summary>
    /// The naming convention to be used when mapping members of the types in this group to database columns.
    /// </summary>
    public DbObjectNameConvention ColumnNaming { get; set; } = new();
}