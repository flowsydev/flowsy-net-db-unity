namespace Flowsy.Db.Unity.Conventions;

public class DbDateTimeConvention : DbConvention
{
    internal DbDateTimeConvention(DbConventionSet conventions) : base(conventions)
    {
    }

    /// <summary>
    /// The format used to handle DateTimeOffset values.
    /// </summary>
    public DbDateTimeOffsetFormat OffsetValueFormat { get; internal set; } = DbDateTimeOffsetFormat.Utc;
    
    public void CopyTo(DbDateTimeConvention other)
    {
        other.OffsetValueFormat = OffsetValueFormat;
    }
    
    public DbDateTimeConvention Clone(DbConventionSet parentConventions)
    {
        var clone = new DbDateTimeConvention(parentConventions);
        CopyTo(clone);
        return clone;
    }
}