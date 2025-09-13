namespace Flowsy.Db.Unity;

public partial interface IDbSession
{
    /// <summary>
    /// Executes a stored procedure or function in the database.
    /// </summary>
    /// <param name="routineName">
    /// Name of the stored procedure or function to execute.
    /// </param>
    /// <param name="parameters">
    /// Dynamic parameters that will be passed to the stored procedure or function.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of executing the stored procedure or function.
    /// </returns>
    Task<int> ExecuteRoutineAsync(
        string routineName,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
        );

    /// <summary>
    /// Executes a stored procedure or function in the database.
    /// </summary>
    /// <param name="routineName">
    /// Name of the stored procedure or function to execute.
    /// </param>
    /// <param name="routineType">
    /// Type of routine: stored procedure or function.
    /// </param>
    /// <param name="parameters">
    /// Dynamic parameters that will be passed to the stored procedure or function.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of executing the stored procedure or function.
    /// </returns>
    Task<int> ExecuteRoutineAsync(
        string routineName,
        DbRoutineType routineType,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
        );
}