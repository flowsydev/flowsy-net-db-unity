namespace Flowsy.Db.Unity;

public partial interface IDbSession
{
    /// <summary>
    /// Executes a SQL statement that does not return results, such as INSERT, UPDATE, or DELETE.
    /// </summary>
    /// <param name="statement">
    /// SQL statement to execute.
    /// </param>
    /// <param name="parameters">
    /// Optional parameters for the statement, which can be used to avoid SQL injections.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of executing the SQL statement.
    /// The result is the number of rows affected by the operation.
    /// </returns>
    Task<int> ExecuteAsync(
        string statement,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
    );
}
