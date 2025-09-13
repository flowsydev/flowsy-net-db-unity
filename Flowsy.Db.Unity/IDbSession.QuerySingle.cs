namespace Flowsy.Db.Unity;

public partial interface IDbSession
{
    /// <summary>
    /// Performs a database query and returns exactly one result.
    /// Throws an exception if no results are found or if multiple results are found.
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
    /// The result is exactly one element returned by the query.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the query returns no results or multiple results.
    /// </exception>
    Task<T> QuerySingleAsync<T>(
        string statement,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Performs a database query and returns exactly one result, or the default value if no results are found.
    /// Throws an exception if multiple results are found.
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
    /// The result is exactly one element returned by the query, or the default value of type T if no results are found.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the query returns multiple results.
    /// </exception>
    Task<T?> QuerySingleOrDefaultAsync<T>(
        string statement,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
    );
}
