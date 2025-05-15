namespace Flowsy.Db.Unity.Conventions;

/// <summary>
/// Base class for defining database conventions.
/// </summary>
public abstract class DbConvention
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DbConvention"/> class.
    /// </summary>
    /// <param name="conventions"></param>
    protected DbConvention(DbConventionSet conventions)
    {
        Conventions = conventions;
    }

    /// <summary>
    /// Gets the set of conventions to which this convention belongs.
    /// </summary>
    public DbConventionSet Conventions { get; }
}