namespace Flowsy.Db.Unity.Conventions;

/// <summary>
/// Specifies the format used to handle enums.
/// </summary>
public enum DbEnumFormat
{
    /// <summary>
    /// The enum value name is used.
    /// </summary>
    Name,
    
    /// <summary>
    /// The enum value ordinal is used.
    /// </summary>
    Ordinal
}