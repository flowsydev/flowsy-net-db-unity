namespace Flowsy.Db.Unity.Conventions;

public class DbDateTimeConventionBuilder : DbConventionBuilder
{
    internal DbDateTimeConventionBuilder(DbConventionSetBuilder parent) : base(parent)
    {
    }

    public DbDateTimeConventionBuilder UseDateTimeOffsets(DbDateTimeOffsetFormat valueFormat)
    {
        Parent.Conventions.DateTime.OffsetValueFormat = valueFormat;
        return this;
    }
}