namespace Flowsy.Db.Unity.Conventions;

/// <summary>
/// Builder for configuring date and time conventions for database operations.
/// </summary>
public class DbDateTimeConventionBuilder : DbConventionBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DbDateTimeConventionBuilder"/> class.
    /// </summary>
    /// <param name="parent">
    /// The parent <see cref="DbConventionSetBuilder"/> instance. This is used to access the parent convention set and apply configurations.
    /// </param>
    internal DbDateTimeConventionBuilder(DbConventionSetBuilder parent) : base(parent)
    {
    }

    /// <summary>
    /// Sets the format for DateTimeOffset values for the target <see cref="DbDateTimeConvention"/> instance.
    /// </summary>
    /// <param name="valueFormat">
    /// The format to be used for DateTimeOffset values. This can be one of the values from the <see cref="DbDateTimeOffsetFormat"/> enumeration.
    /// </param>
    /// <returns>
    /// A reference to the current <see cref="DbDateTimeConventionBuilder"/> instance, allowing for method chaining.
    /// </returns>
    public DbDateTimeConventionBuilder UseDateTimeOffsets(DbDateTimeOffsetFormat valueFormat)
    {
        Parent.Conventions.DateTime.OffsetValueFormat = valueFormat;
        return this;
    }
}