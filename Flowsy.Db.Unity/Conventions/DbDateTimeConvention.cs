namespace Flowsy.Db.Unity.Conventions;

/// <summary>
/// Represents conventions for DateTime and DateTimeOffset types in the database.
/// </summary>
/// <param name="OffsetValueFormat">The format to use for DateTimeOffset values when stored in the database.</param>
public record DbDateTimeConvention(DbDateTimeOffsetFormat OffsetValueFormat) : DbConvention
{
    /// <summary>
    /// Default DateTime convention that uses UTC format for DateTimeOffset values.
    /// </summary>
    public static readonly DbDateTimeConvention Default = new(DbDateTimeOffsetFormat.Utc);
}