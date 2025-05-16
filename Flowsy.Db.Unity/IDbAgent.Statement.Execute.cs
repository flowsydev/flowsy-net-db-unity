namespace Flowsy.Db.Unity;

public partial interface IDbAgent
{
    /// <summary>
    /// Executes a SQL statement with the specified text and parameters. 
    /// </summary>
    /// <param name="commandText">
    /// The text of the SQL statement to execute.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the statement. This can be an anonymous object or a dictionary.
    /// </param>
    /// <returns>
    /// The number of rows affected by the statement execution.
    /// </returns>
    int ExecuteStatement(string commandText, dynamic? parameters = null);

    /// <summary>
    /// Asynchronously executes a SQL statement with the specified text and parameters. 
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
    /// <returns>
    /// The number of rows affected by the statement execution.
    /// </returns>
    Task<int> ExecuteStatementAsync(string commandText, dynamic? parameters = null, CancellationToken cancellationToken = default);
}