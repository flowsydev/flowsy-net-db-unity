namespace Flowsy.Db.Unity;

/// <summary>
/// Represents a database object, such as a table, view, routine, parameter, etc.
/// </summary>
public abstract class DbObjectDescriptor
{
    /// <summary>
    /// Creates an instance of <see cref="DbObjectDescriptor"/> with the fully qualified name of the database object.
    /// </summary>
    /// <param name="fullyQualifiedName">
    /// The fully qualified name of the database object.
    /// </param>
    protected DbObjectDescriptor(DbFullyQualifiedName fullyQualifiedName)
    {
        FullyQualifiedName = fullyQualifiedName;
    }
    
    /// <summary>
    /// The database provider for the object.
    /// </summary>
    public DbProviderDescriptor Provider => FullyQualifiedName.Provider;
    
    /// <summary>
    /// The fully qualified name of the database object.
    /// </summary>
    public DbFullyQualifiedName FullyQualifiedName { get; }
}