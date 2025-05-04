namespace Flowsy.Db.Unity.Conventions;

public class DbEnumMapping
{
    public DbEnumMapping(Type runtimeType, string databaseTypeName, DbConventionSet conventions)
        : this(runtimeType, conventions.Provider.ParseObjectName(databaseTypeName), conventions)
    {
    }
        
    public DbEnumMapping(Type runtimeType, DbFullyQualifiedName databaseTypeName, DbConventionSet conventions)
    {
        RuntimeType = runtimeType;
        DatabaseTypeName = databaseTypeName;
        Conventions = conventions;
    }

    public Type RuntimeType { get; }
    public DbFullyQualifiedName DatabaseTypeName { get; }
    
    public DbConventionSet Conventions { get; }
}

public class DbEnumMapping<TEnum> : DbEnumMapping
    where TEnum : struct, Enum
{
    public DbEnumMapping(string databaseTypeName, DbConventionSet conventions) 
        : base(typeof(TEnum), databaseTypeName, conventions)
    {
    }
}