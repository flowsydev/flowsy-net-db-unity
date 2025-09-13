namespace Flowsy.Db.Unity;

/// <summary>
/// Represents a fully qualified name for a database object.
/// </summary>
public record DbFullyQualifiedName
{
    /// <summary>
    /// Creates a new instance of the <see cref="DbFullyQualifiedName"/> class.
    /// </summary>
    /// <param name="provider">
    /// The database provider descriptor for this fully qualified name.
    /// </param>
    /// <param name="parts">
    /// The parts of the fully qualified name of the database object.
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
    /// Creates a new instance of the <see cref="DbFullyQualifiedName"/> class from a child name and additional parts.
    /// </summary>
    /// <param name="child">
    /// The fully qualified name of the child object.
    /// </param>
    /// <param name="parts">
    /// The parts of the fully qualified name of the database object.
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
    /// The parts of the fully qualified name of the database object.
    /// </summary>
    public IEnumerable<string> Parts { get; }
    
    /// <summary>
    /// The simple name of the database object.
    /// </summary>
    public string SimpleName => Parts.Last();
    
    /// <summary>
    /// The fully qualified name of the parent object, if it exists.
    /// </summary>
    public DbFullyQualifiedName? Parent { get; }
    
    /// <summary>
    /// The fully qualified name of the child object, if it exists.
    /// </summary>
    public DbFullyQualifiedName? Child { get; }
    
    /// <summary>
    /// Returns a string representation of the fully qualified name of the database object.
    /// </summary>
    /// <returns>
    /// A string that represents the fully qualified name of the database object.
    /// </returns>
    public override string ToString() => string.Join(Provider.ObjectSeparator, Parts);
}