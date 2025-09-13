namespace Flowsy.Db.Unity;

/// <summary>
/// Represents the usage of a database connection.
/// </summary>
public enum DbConnectionUsage
{
    /// <summary>
    /// Indicates that the database connection is shared within the current dependency injection scope.
    /// </summary>
    Shared,
    
    /// <summary>
    /// Indicates that the database connection is exclusive to a single process or task.
    /// </summary>
    Exclusive
}