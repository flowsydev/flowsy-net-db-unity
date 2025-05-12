namespace Flowsy.Db.Unity;

/// <summary>
/// Available formats to handle DateTimeOffset values.
/// </summary>
public enum DbDateTimeOffsetFormat
{
    /// <summary>
    /// Central time zone
    /// </summary>
    Utc,
    
    /// <summary>
    /// Local time zone
    /// </summary>
    Local
}