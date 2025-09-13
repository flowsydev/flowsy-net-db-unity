namespace Flowsy.Db.Unity;

/// <summary>
/// Represents a call to a database routine (stored procedure or function) with its SQL statement and parameters.
/// </summary>
/// <param name="Statement">
/// The SQL statement that represents the call to the database routine.
/// </param>
/// <param name="ParameterBuilder">
/// The parameter builder that contains the parameters needed to execute the routine.
/// </param>
public record DbRoutineCall(string Statement, DbParameterBuilder ParameterBuilder);