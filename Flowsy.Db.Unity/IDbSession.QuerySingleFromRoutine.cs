namespace Flowsy.Db.Unity;

public partial interface IDbSession
{
    /// <summary>
    /// Performs a query to a stored procedure or function in the database and returns exactly one result.
    /// Throws an exception if no results are found or if multiple results are found.
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
    /// <typeparam name="T">
    /// Type of expected result from the query.
    /// </typeparam>
    /// <returns>
    /// A task that represents the asynchronous database query operation.
    /// The result is exactly one element returned by the query.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the query returns no results or multiple results.
    /// </exception>
    Task<T> QuerySingleFromRoutineAsync<T>(
        string routineName,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Performs a query to a stored procedure or function in the database and returns exactly one result.
    /// Throws an exception if no results are found or if multiple results are found.
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
    /// <typeparam name="T">
    /// Type of expected result from the query.
    /// </typeparam>
    /// <returns>
    /// A task that represents the asynchronous database query operation.
    /// The result is exactly one element returned by the query.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the query returns no results or multiple results.
    /// </exception>
    Task<T> QuerySingleFromRoutineAsync<T>(
        string routineName,
        DbRoutineType routineType,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Performs a query to a stored procedure or function in the database and returns exactly one result, or the default value if no results are found.
    /// Throws an exception if multiple results are found.
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
    /// <typeparam name="T">
    /// Type of expected result from the query.
    /// </typeparam>
    /// <returns>
    /// A task that represents the asynchronous database query operation.
    /// The result is exactly one element returned by the query, or the default value of type T if no results are found.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the query returns multiple results.
    /// </exception>
    Task<T?> QuerySingleOrDefaultFromRoutineAsync<T>(
        string routineName,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Performs a query to a stored procedure or function in the database and returns exactly one result, or the default value if no results are found.
    /// Throws an exception if multiple results are found.
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
    /// <typeparam name="T">
    /// Type of expected result from the query.
    /// </typeparam>
    /// <returns>
    /// A task that represents the asynchronous database query operation.
    /// The result is exactly one element returned by the query, or the default value of type T if no results are found.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the query returns multiple results.
    /// </exception>
    Task<T?> QuerySingleOrDefaultFromRoutineAsync<T>(
        string routineName,
        DbRoutineType routineType,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
    );
}
