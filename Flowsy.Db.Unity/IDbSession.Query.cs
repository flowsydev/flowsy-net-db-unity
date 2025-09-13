namespace Flowsy.Db.Unity;

public partial interface IDbSession
{
    /// <summary>
    /// Performs a database query and returns an enumerable of results.
    /// </summary>
    /// <param name="statement">
    /// SQL statement to execute.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <typeparam name="T">
    /// Type of expected results from the query.
    /// </typeparam>
    /// <returns>
    /// A task that represents the asynchronous database query operation.
    /// </returns>
    Task<IEnumerable<T>> QueryAsync<T>(
        string statement,
        CancellationToken cancellationToken = default
        );
    
    /// <summary>
    /// Performs a database query and returns an enumerable of results.
    /// </summary>
    /// <param name="statement">
    /// SQL statement to execute.
    /// </param>
    /// <param name="parameters">
    /// Optional parameters for the query, which can be used to avoid SQL injections.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <typeparam name="T">
    /// Type of expected results from the query.
    /// </typeparam>
    /// <returns>
    /// A task that represents the asynchronous database query operation.
    /// </returns>
    Task<IEnumerable<T>> QueryAsync<T>(
        string statement,
        dynamic? parameters,
        CancellationToken cancellationToken = default
        );
    
    /// <summary>
    /// Performs a query to a stored procedure or function in the database and returns an enumerable of results.
    /// </summary>
    /// <param name="routineName">
    /// Name of the stored procedure or function to execute.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <typeparam name="T">
    /// Type of expected results from the query.
    /// </typeparam>
    /// <returns>
    /// A task that represents the asynchronous database query operation.
    /// </returns>
    Task<IEnumerable<T>> QueryFromRoutineAsync<T>(
        string routineName,
        CancellationToken cancellationToken = default
        );
    
    /// <summary>
    /// Performs a query to a stored procedure or function in the database and returns an enumerable of results.
    /// </summary>
    /// <param name="routineName">
    /// Name of the stored procedure or function to execute.
    /// </param>
    /// <param name="parameters">
    /// Optional parameters for the query.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <typeparam name="T">
    /// Type of expected results from the query.
    /// </typeparam>
    /// <returns>
    /// A task that represents the asynchronous database query operation.
    /// </returns>
    Task<IEnumerable<T>> QueryFromRoutineAsync<T>(
        string routineName,
        dynamic? parameters,
        CancellationToken cancellationToken = default
        );

    /// <summary>
    /// Performs a query to a stored procedure or function in the database and returns an enumerable of results.
    /// </summary>
    /// <param name="routineName">
    /// Name of the stored procedure or function to execute.
    /// </param>
    /// <param name="routineType">
    /// Type of routine (stored procedure or function).
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <typeparam name="T">
    /// Type of expected results from the query.
    /// </typeparam>
    /// <returns>
    /// A task that represents the asynchronous database query operation.
    /// </returns>
    Task<IEnumerable<T>> QueryFromRoutineAsync<T>(
        string routineName,
        DbRoutineType routineType,
        CancellationToken cancellationToken = default
        );

    /// <summary>
    /// Performs a query to a stored procedure or function in the database and returns an enumerable of results.
    /// </summary>
    /// <param name="routineName">
    /// Name of the stored procedure or function to execute.
    /// </param>
    /// <param name="routineType">
    /// Type of routine (stored procedure or function).
    /// </param>
    /// <param name="parameters">
    /// Optional parameters for the query.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <typeparam name="T">
    /// Type of expected results from the query.
    /// </typeparam>
    /// <returns>
    /// A task that represents the asynchronous database query operation.
    /// </returns>
    Task<IEnumerable<T>> QueryFromRoutineAsync<T>(
        string routineName,
        DbRoutineType routineType,
        dynamic? parameters,
        CancellationToken cancellationToken = default
        );
}