using Flowsy.Db.Unity.Extensions;

namespace Flowsy.Db.Unity;

public partial class DbAgent
{
    /// <summary>
    /// Executes a database statement (SQL command) asynchronously and returns the result as an enumerable collection of type T. 
    /// </summary>
    /// <param name="commandText">
    /// The text of the SQL statement to execute.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the statement. This can be an anonymous object or a dictionary.
    /// </param>
    /// <typeparam name="T">
    /// The type to map the results to.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of type T containing the result set returned by the statement.
    /// </returns>
    public IEnumerable<T> GetFromStatement<T>(string commandText, dynamic? parameters = null)
        => Connection.GetFromStatement<T>(
            commandText,
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
    /// Asynchronously executes a database statement (SQL command) asynchronously and returns the result as an enumerable collection of type T. 
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
    /// The type to map the results to.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of type T containing the result set returned by the statement.
    /// </returns>
    public Task<IEnumerable<T>> GetFromStatementAsync<T>(
        string commandText,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
    )
        => Connection.GetFromStatementAsync<T>(
            commandText,
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