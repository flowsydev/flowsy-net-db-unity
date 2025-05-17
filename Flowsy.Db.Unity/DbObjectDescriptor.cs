namespace Flowsy.Db.Unity;

/// <summary>
/// Abstract representation of a database object descriptor.
/// </summary>
public abstract class DbObjectDescriptor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DbObjectDescriptor"/> class with the specified fully qualified name.
    /// </summary>
    /// <param name="fullyQualifiedName">
    /// The fully qualified name of the database object.
    /// </param>
    protected DbObjectDescriptor(DbFullyQualifiedName fullyQualifiedName)
    {
        FullyQualifiedName = fullyQualifiedName;
    }
    
    /// <summary>
    /// The provider descriptor for the database object.
    /// </summary>
    public DbProviderDescriptor Provider => FullyQualifiedName.Provider;
    
    /// <summary>
    /// The fully qualified name of the database object.
    /// </summary>
    public DbFullyQualifiedName FullyQualifiedName { get; }
}