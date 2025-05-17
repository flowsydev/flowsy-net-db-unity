namespace Flowsy.Db.Unity;

/// <summary>
/// Represents a fully qualified name for a database object.
/// </summary>
public class DbFullyQualifiedName
{
    /// <summary>
    /// Creates a new instance of <see cref="DbFullyQualifiedName"/>.
    /// </summary>
    /// <param name="provider">
    /// The database provider descriptor.
    /// </param>
    /// <param name="parts">
    /// The parts of the fully qualified name.
    /// </param>
    public DbFullyQualifiedName(DbProviderDescriptor provider, params string[] parts)
    {
        Provider = provider;
        Parts = parts;
        if (parts.Length < 2) return;
        
        var parentParts = parts.Take(parts.Length - 1).ToArray();
        if (parentParts.Length > 0)
            Parent = new DbFullyQualifiedName(this, parentParts);
    }
    
    /// <summary>
    /// Creates a new instance of <see cref="DbFullyQualifiedName"/> with a child.
    /// </summary>
    /// <param name="child">
    /// The child <see cref="DbFullyQualifiedName"/> instance.
    /// </param>
    /// <param name="parts">
    /// The parts of the fully qualified name.
    /// </param>
    public DbFullyQualifiedName(DbFullyQualifiedName child, params string[] parts)
    {
        Provider = child.Provider;
        Parts = parts;
        Child = child;
        if (parts.Length < 2) return;
        
        var parentParts = parts.Take(parts.Length - 1).ToArray();
        if (parentParts.Length > 0)
            Parent = new DbFullyQualifiedName(this, parentParts);
    }

    /// <summary>
    /// The database provider descriptor for this fully qualified name.
    /// </summary>
    public DbProviderDescriptor Provider { get; }
    
    /// <summary>
    /// The parts of the fully qualified name.
    /// </summary>
    public IEnumerable<string> Parts { get; }
    
    /// <summary>
    /// The last part of the fully qualified name.
    /// </summary>
    public string SimpleName => Parts.Last();
    
    /// <summary>
    /// The parent fully qualified name, if any.
    /// </summary>
    public DbFullyQualifiedName? Parent { get; }
    
    /// <summary>
    /// The child fully qualified name, if any.
    /// </summary>
    public DbFullyQualifiedName? Child { get; }

    /// <summary>
    /// Returns a string representation of the fully qualified name.
    /// </summary>
    /// <returns>
    /// A string representation of the fully qualified name.
    /// </returns>
    public override string ToString() => string.Join(Provider.ObjectSeparator, Parts);
}