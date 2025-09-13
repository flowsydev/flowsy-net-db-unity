namespace Flowsy.Db.Unity;

/// <summary>
/// Represents the type of database routine.
/// </summary>
public enum DbRoutineType
{
    /// <summary>
    /// Stored procedure.
    /// </summary>
    StoredProcedure,
    
    /// <summary>
    /// Stored function.
    /// </summary>
    StoredFunction,
}