using Dapper;

namespace Flowsy.Db.Unity;

public partial interface IDbSession
{
    /// <summary>
    /// Performs a query to a stored procedure or function in the database that can return multiple result sets.
    /// </summary>
    /// <param name="routineName">
    /// Name of the stored procedure or function to execute.
    /// </param>
    /// <param name="parameters">
    /// Optional parameters for the routine.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous database query operation.
    /// The result is a GridReader object that allows reading multiple result sets.
    /// </returns>
    Task<SqlMapper.GridReader> QueryMultipleFromRoutineAsync(
        string routineName,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Performs a query to a stored procedure or function in the database that can return multiple result sets.
    /// </summary>
    /// <param name="routineName">
    /// Name of the stored procedure or function to execute.
    /// </param>
    /// <param name="routineType">
    /// Type of routine (stored procedure or function).
    /// </param>
    /// <param name="parameters">
    /// Optional parameters for the routine.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous database query operation.
    /// The result is a GridReader object that allows reading multiple result sets.
    /// </returns>
    Task<SqlMapper.GridReader> QueryMultipleFromRoutineAsync(
        string routineName,
        DbRoutineType routineType,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>Consumes a routine's result sets in a callback and disposes the reader.</summary>
    Task QueryMultipleFromRoutineAsync(
        string routineName,
        DbRoutineType routineType,
        Func<SqlMapper.GridReader, CancellationToken, Task> read,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default);

    /// <summary>Consumes a routine's result sets in a callback, returns a result, and disposes the reader.</summary>
    Task<TResult> QueryMultipleFromRoutineAsync<TResult>(
        string routineName,
        DbRoutineType routineType,
        Func<SqlMapper.GridReader, CancellationToken, Task<TResult>> read,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default);
}
