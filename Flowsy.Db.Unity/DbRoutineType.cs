namespace Flowsy.Db.Unity;

/// <summary>
/// Represents the type of a database routine.
/// </summary>
public enum DbRoutineType
{
    /// <summary>
    ///  A stored procedure.
    /// </summary>
    Procedure,
    
    /// <summary>
    ///  A stored function.
    /// </summary>
    Function
}