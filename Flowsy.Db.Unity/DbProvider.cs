using System.Data.Common;
using Flowsy.Db.Unity.Resources;

namespace Flowsy.Db.Unity;

public class DbProvider
{
    public static readonly DbProvider Generic = new (DbProviderFamily.Generic);
    
    public DbProvider(DbProviderFamily family, string? invariantName = null, DbProviderFactory? factory = null)
    {
        Family = family;
        InvariantName = invariantName;
        Factory = factory;
    }

    public DbProviderFamily Family { get; }
    public string? InvariantName { get; }
    public DbProviderFactory? Factory { get; }

    public string ObjectSeparator => Family switch
    {
        DbProviderFamily.Generic => ".",
        DbProviderFamily.Postgres => ".",
        DbProviderFamily.MySql => ".",
        DbProviderFamily.SqlServer => ".",
        DbProviderFamily.Oracle => ".",
        DbProviderFamily.Sqlite => ".",
        _ => throw new NotSupportedException(string.Format(Strings.ProviderXNotSupported, Family.ToString()))
    };
    
    public DbFullyQualifiedName ParseObjectName(string name) => new (this, name.Split(ObjectSeparator));
}