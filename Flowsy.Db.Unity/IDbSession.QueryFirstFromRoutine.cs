namespace Flowsy.Db.Unity;

public partial interface IDbSession
{
    /// <summary>
    /// Performs a query to a stored procedure or function in the database and returns the first result.
    /// Throws an exception if no results are found.
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
    /// The result is the first element returned by the query.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the query returns no results.
    /// </exception>
    Task<T> QueryFirstFromRoutineAsync<T>(
        string routineName,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Performs a query to a stored procedure or function in the database and returns the first result.
    /// Throws an exception if no results are found.
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
    /// The result is the first element returned by the query.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the query returns no results.
    /// </exception>
    Task<T> QueryFirstFromRoutineAsync<T>(
        string routineName,
        DbRoutineType routineType,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Performs a query to a stored procedure or function in the database and returns the first result, or the default value if no results are found.
    /// Returns the default value if no results are found.
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
    /// The result is the first element returned by the query, or the default value of type T if no results are found.
    /// </returns>
    Task<T?> QueryFirstOrDefaultFromRoutineAsync<T>(
        string routineName,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Performs a query to a stored procedure or function in the database and returns the first result, or the default value if no results are found.
    /// Returns the default value if no results are found.
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
    /// The result is the first element returned by the query, or the default value of type T if no results are found.
    /// </returns>
    Task<T?> QueryFirstOrDefaultFromRoutineAsync<T>(
        string routineName,
        DbRoutineType routineType,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
    );
}
