namespace Flowsy.Db.Unity;

/// <summary>
/// Specifies the format of enum values when stored in the database.
/// </summary>
public enum DbEnumValueFormat
{
    /// <summary>
    /// Enum values are stored using their string name.
    /// </summary>
    Name,
    
    /// <summary>
    /// Enum values are stored using their ordinal (numeric) value.
    /// </summary>
    Ordinal
}