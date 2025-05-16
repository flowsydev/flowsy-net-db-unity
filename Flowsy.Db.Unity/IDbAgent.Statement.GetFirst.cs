namespace Flowsy.Db.Unity;

public partial interface IDbAgent
{
    /// <summary>
    /// Executes a database statement (SQL command) and returns the first result as an instance of type T.
    /// If a primitive type is required (int, string, etc.), then the first column of the first result will be returned.
    /// If no results are found, an exception will be thrown.
    /// </summary>
    /// <param name="commandText">
    /// The text of the SQL statement to execute.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the statement. This can be an anonymous object or a dictionary.
    /// </param>
    /// <typeparam name="T">
    /// The type to map the result to.
    /// </typeparam>
    /// <returns>
    /// The first result of the statement execution as an instance of type T.
    /// </returns>
    T GetFirstFromStatement<T>(string commandText, dynamic? parameters = null);

    /// <summary>
    /// Asynchronously executes a database statement (SQL command) and returns the first result as an instance of type T.
    /// If a primitive type is required (int, string, etc.), then the first column of the first result will be returned.
    /// If no results are found, an exception will be thrown.
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
    /// The type to map the result to.
    /// </typeparam>
    /// <returns>
    /// The first result of the statement execution as an instance of type T.
    /// </returns>
    Task<T> GetFirstFromStatementAsync<T>(string commandText, dynamic? parameters = null, CancellationToken cancellationToken = default);
}