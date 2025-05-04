namespace Flowsy.Db.Unity;

public class DbFullyQualifiedName
{
    public DbFullyQualifiedName(DbProvider provider, params string[] parts)
    {
        Provider = provider;
        Parts = parts;
        if (parts.Length < 2) return;
        
        var parentParts = parts.Take(parts.Length - 1).ToArray();
        if (parentParts.Length > 0)
            Parent = new DbFullyQualifiedName(this, parentParts);
    }
    
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

    public DbProvider Provider { get; }
    public IEnumerable<string> Parts { get; }
    
    public string SimpleName => Parts.Last();
    public DbFullyQualifiedName? Parent { get; }
    public DbFullyQualifiedName? Child { get; }

    public override string ToString() => string.Join(Provider.ObjectSeparator, Parts);
}