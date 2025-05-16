namespace Flowsy.Db.Unity;

public partial interface IDbAgent
{
    /// <summary>
    /// Executes a SQL script from a file.
    /// </summary>
    /// <param name="scriptPath">
    /// The path to the SQL script file to execute.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the SQL script. This can be an anonymous object or a dictionary.
    /// </param>
    /// <returns>
    /// The number of rows affected by the script execution.
    /// </returns>
    int ExecuteScript(string scriptPath, dynamic? parameters = null);

    /// <summary>
    /// Executes a SQL script from a stream.
    /// </summary>
    /// <param name="scriptStream">
    /// The stream containing the SQL script to execute.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the SQL script. This can be an anonymous object or a dictionary.
    /// </param>
    /// <returns>
    /// The number of rows affected by the script execution.
    /// </returns>
    int ExecuteScript(Stream scriptStream, dynamic? parameters = null);

    /// <summary>
    /// Asynchronously executes a SQL script from a file.
    /// </summary>
    /// <param name="scriptPath">
    /// The path to the SQL script file to execute.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the SQL script. This can be an anonymous object or a dictionary.
    /// </param>
    /// <param name="cancellationToken">
    /// An optional cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// The number of rows affected by the script execution.
    /// </returns>
    Task<int> ExecuteScriptAsync(string scriptPath, dynamic? parameters = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously executes a SQL script from a stream.
    /// </summary>
    /// <param name="scriptStream">
    /// The stream containing the SQL script to execute.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the SQL script. This can be an anonymous object or a dictionary.
    /// </param>
    /// <param name="cancellationToken">
    /// An optional cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// The number of rows affected by the script execution.
    /// </returns>
    Task<int> ExecuteScriptAsync(Stream scriptStream, dynamic? parameters = null, CancellationToken cancellationToken = default);
}