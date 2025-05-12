using Flowsy.Db.Unity.Resources;

namespace Flowsy.Db.Unity.Conventions;

public class DbEnumMapping
{
    public DbEnumMapping(Type runtimeType, string? databaseTypeName, DbEnumNameTranslator? nameTranslator, DbConventionSet conventions)
        : this(runtimeType, string.IsNullOrEmpty(databaseTypeName) ? null : conventions.Provider.ParseObjectName(databaseTypeName), nameTranslator, conventions)
    {
    }
        
    public DbEnumMapping(Type runtimeType, DbFullyQualifiedName? databaseTypeName, DbEnumNameTranslator? nameTranslator, DbConventionSet conventions)
    {
        if (!runtimeType.IsEnum)
            throw new ArgumentException(string.Format(Strings.TypeXIsNotAnEnumType, runtimeType.FullName), nameof(runtimeType));
        
        RuntimeType = runtimeType;
        DatabaseTypeName = databaseTypeName;
        NameTranslator = nameTranslator;
        Conventions = conventions;
    }

    public Type RuntimeType { get; }
    public DbFullyQualifiedName? DatabaseTypeName { get; }
    public DbEnumNameTranslator? NameTranslator { get; }
    public DbConventionSet Conventions { get; }
}

public class DbEnumMapping<TEnum> : DbEnumMapping
    where TEnum : struct, Enum
{
    public DbEnumMapping(string? databaseTypeName, DbEnumNameTranslator? nameTranslator, DbConventionSet conventions) 
        : base(typeof(TEnum), databaseTypeName, nameTranslator, conventions)
    {
    }
    
    public DbEnumMapping(DbFullyQualifiedName? databaseTypeName, DbEnumNameTranslator? nameTranslator, DbConventionSet conventions) 
        : base(typeof(TEnum), databaseTypeName, nameTranslator, conventions)
    {
    }
}