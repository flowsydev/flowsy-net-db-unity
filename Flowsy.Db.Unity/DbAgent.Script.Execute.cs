using Flowsy.Db.Unity.Extensions;

namespace Flowsy.Db.Unity;

public partial class DbAgent
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
    public int ExecuteScript(string scriptPath, dynamic? parameters = null)
        => Connection.ExecuteScript(
            scriptPath,
            parameters as object,
            UnitOfWork?.Transaction,
            ConnectionOptions.Conventions,
            c => OnCommandExecuting(new DbCommandExecutingEventArgs(c, Connection, UnitOfWork?.Transaction)),
            (c, r) =>
            {
                OnCommandExecuted(new DbCommandExecutedEventArgs(c, Connection, UnitOfWork?.Transaction, r));
                return r;
            }
        );

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
    public int ExecuteScript(Stream scriptStream, dynamic? parameters = null)
        => Connection.ExecuteScript(
            scriptStream,
            parameters as object,
            UnitOfWork?.Transaction,
            ConnectionOptions.Conventions,
            c => OnCommandExecuting(new DbCommandExecutingEventArgs(c, Connection, UnitOfWork?.Transaction)),
            (c, r) =>
            {
                OnCommandExecuted(new DbCommandExecutedEventArgs(c, Connection, UnitOfWork?.Transaction, r));
                return r;
            }
            );
    
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
    public Task<int> ExecuteScriptAsync(string scriptPath, dynamic? parameters = null, CancellationToken cancellationToken = default)
        => Connection.ExecuteScriptAsync(
            scriptPath,
            parameters as object,
            UnitOfWork?.Transaction,
            ConnectionOptions.Conventions,
            c => OnCommandExecuting(new DbCommandExecutingEventArgs(c, Connection, UnitOfWork?.Transaction)),
            (c, r) =>
            {
                OnCommandExecuted(new DbCommandExecutedEventArgs(c, Connection, UnitOfWork?.Transaction, r));
                return Task.FromResult(r);
            },
            cancellationToken
        );

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
    public Task<int> ExecuteScriptAsync(Stream scriptStream, dynamic? parameters = null, CancellationToken cancellationToken = default)
        => Connection.ExecuteScriptAsync(
            scriptStream,
            parameters as object,
            UnitOfWork?.Transaction,
            ConnectionOptions.Conventions,
            c => OnCommandExecuting(new DbCommandExecutingEventArgs(c, Connection, UnitOfWork?.Transaction)),
            (c, r) =>
            {
                OnCommandExecuted(new DbCommandExecutedEventArgs(c, Connection, UnitOfWork?.Transaction, r));
                return Task.FromResult(r);
            },
            cancellationToken
            );
}