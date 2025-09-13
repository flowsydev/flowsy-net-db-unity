namespace Flowsy.Db.Unity;

public partial interface IDbSession
{
    /// <summary>
    /// Executes a SQL script from a file or directory.
    /// If the `scriptPath` parameter is a directory, all files with `.sql` extension will be executed in alphabetical order.
    /// </summary>
    /// <param name="scriptPath">
    /// Path to the file or directory containing the SQL script to execute.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of executing the SQL script.
    /// </returns>
    Task ExecuteScriptAsync(string scriptPath, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes a SQL script from a data stream.
    /// </summary>
    /// <param name="scriptStream">
    /// Data stream containing the SQL script to execute.
    /// </param>
    /// <param name="filePath">
    /// File path, if applicable, from which the script was loaded, used for logging purposes.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of executing the SQL script from the data stream.
    /// </returns>
    Task ExecuteScriptAsync(Stream scriptStream, string? filePath = null, CancellationToken cancellationToken = default);
}