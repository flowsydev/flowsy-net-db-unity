namespace Flowsy.Db.Unity.Conventions;

/// <summary>
/// Represents a convention for handling DateTime and DateTimeOffset values in database operations.
/// </summary>
public class DbDateTimeConvention : DbConvention
{
    internal DbDateTimeConvention(DbConventionSet conventions) : base(conventions)
    {
    }

    /// <summary>
    /// The format used to handle DateTimeOffset values.
    /// </summary>
    public DbDateTimeOffsetFormat OffsetValueFormat { get; internal set; } = DbDateTimeOffsetFormat.Utc;
    
    /// <summary>
    /// Copies the properties of this <see cref="DbDateTimeConvention"/> instance to another instance.
    /// </summary>
    /// <param name="other">
    /// The other <see cref="DbDateTimeConvention"/> instance to copy properties to.
    /// </param>
    public void CopyTo(DbDateTimeConvention other)
    {
        other.OffsetValueFormat = OffsetValueFormat;
    }
    
    /// <summary>
    /// Creates a clone of this <see cref="DbDateTimeConvention"/> instance.
    /// </summary>
    /// <param name="parentConventions">
    /// The parent <see cref="DbConventionSet"/> to which the cloned convention will belong.
    /// </param>
    /// <returns>
    /// A new instance of <see cref="DbDateTimeConvention"/> with the same properties as this instance.
    /// </returns>
    public DbDateTimeConvention Clone(DbConventionSet parentConventions)
    {
        var clone = new DbDateTimeConvention(parentConventions);
        CopyTo(clone);
        return clone;
    }
}