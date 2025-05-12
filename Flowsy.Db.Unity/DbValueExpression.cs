namespace Flowsy.Db.Unity;

/// <summary>
/// Indicates how a value should be passed to the database.
/// </summary>
public enum DbValueExpression
{
    /// <summary>
    /// The value should be passed as is.
    /// </summary>
    Raw,
    
    /// <summary>
    /// The value should be cast to the database type.
    /// </summary>
    DatabaseTypeCast,
    
    /// <summary>
    /// The value should be cast to a custom type.
    /// </summary>
    CustomTypeCast
}