namespace Flowsy.Db.Unity;

/// <summary>
/// Specifies the time zone format for DateTimeOffset values when stored in the database.
/// </summary>
public enum DbDateTimeOffsetFormat
{
    /// <summary>
    /// DateTimeOffset values are converted and stored in UTC time.
    /// </summary>
    Utc,
    
    /// <summary>
    /// DateTimeOffset values are stored in local time preserving the time zone.
    /// </summary>
    Local
}