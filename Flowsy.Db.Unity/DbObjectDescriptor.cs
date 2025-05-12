namespace Flowsy.Db.Unity;

public abstract class DbObjectDescriptor
{
    protected DbObjectDescriptor(DbFullyQualifiedName fullyQualifiedName)
    {
        FullyQualifiedName = fullyQualifiedName;
    }
    
    public DbProviderDescriptor Provider => FullyQualifiedName.Provider;
    public DbFullyQualifiedName FullyQualifiedName { get; }
}