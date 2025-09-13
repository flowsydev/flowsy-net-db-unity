namespace Flowsy.Db.Unity;

/// <summary>
/// Specifies the different levels of support for enum types in the database.
/// </summary>
public enum DbEnumSupport
{
    /// <summary>
    /// No support for enum types. Values are treated as primitive types.
    /// </summary>
    None,
    
    /// <summary>
    /// Limited support through field restrictions that validate allowed values.
    /// </summary>
    FieldRestriction,
    
    /// <summary>
    /// Full support through native custom types in the database.
    /// </summary>
    CustomType,
}