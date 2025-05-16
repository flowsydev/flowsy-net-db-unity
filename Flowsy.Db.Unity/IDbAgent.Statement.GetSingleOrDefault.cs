namespace Flowsy.Db.Unity;

public partial interface IDbAgent
{
    /// <summary>
    /// Executes a database statement (SQL command) and returns a single result as an instance of T.
    /// If a primitive type is required (int, string, etc.), then the first column of the single result found will be returned.
    /// If no results are found, the default value of T will be returned.
    /// If multiple results are found, an exception will be thrown.
    /// </summary>
    /// <param name="commandText">
    /// The text of the SQL statement to execute.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the statement. This can be an anonymous object or a dictionary.
    /// </param>
    /// <typeparam name="T">
    /// The type of the result to return. This can be a primitive type or a complex type.
    /// </typeparam>
    /// <returns>
    /// The single result of the statement execution as an instance of T.
    /// </returns>
    T? GetSingleOrDefaultFromStatement<T>(string commandText, dynamic? parameters = null);

    /// <summary>
    /// Asynchronously executes a database statement (SQL command) and returns a single result as an instance of T.
    /// If a primitive type is required (int, string, etc.), then the first column of the single result found will be returned.
    /// If no results are found, the default value of T will be returned.
    /// If multiple results are found, an exception will be thrown.
    /// </summary>
    /// <param name="commandText">
    /// The text of the SQL statement to execute.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the statement. This can be an anonymous object or a dictionary.
    /// </param>
    /// <param name="cancellationToken">
    /// An optional cancellation token for the asynchronous operation.
    /// </param>
    /// <typeparam name="T">
    /// The type of the result to return. This can be a primitive type or a complex type.
    /// </typeparam>
    /// <returns>
    /// The single result of the statement execution as an instance of T.
    /// </returns>
    Task<T?> GetSingleOrDefaultFromStatementAsync<T>(string commandText, dynamic? parameters = null, CancellationToken cancellationToken = default);
}