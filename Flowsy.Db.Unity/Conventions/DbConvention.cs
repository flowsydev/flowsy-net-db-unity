namespace Flowsy.Db.Unity.Conventions;

/// <summary>
/// Represents the abstract base class for all database conventions.
/// </summary>
public abstract record DbConvention
{
    /// <summary>
    /// Gets or initializes the convention set to which this convention belongs.
    /// </summary>
    public DbConventionSet? ConventionSet { get; init; }
}