namespace Flowsy.Db.Unity;

public partial interface IDbSession
{
    /// <summary>
    /// Performs a database query and returns the first result.
    /// Throws an exception if no results are found.
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
    /// Type of expected result from the query.
    /// </typeparam>
    /// <returns>
    /// A task that represents the asynchronous database query operation.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no results are found in the query.
    /// </exception>
    Task<T> QueryFirstAsync<T>(
        string statement,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Performs a database query and returns the first result or the default value of the type.
    /// Returns the default value if no results are found.
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
    /// Type of expected result from the query.
    /// </typeparam>
    /// <returns>
    /// A task that represents the asynchronous database query operation.
    /// </returns>
    Task<T?> QueryFirstOrDefaultAsync<T>(
        string statement,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
    );
}
