using Dapper;
using Flowsy.Db.Unity.Extensions;

namespace Flowsy.Db.Unity;

public partial class DbAgent
{
    /// <summary>
    /// Executes a database statement (SQL command) that returns multiple result sets.
    /// </summary>
    /// <param name="commandText">
    /// The text of the SQL statement to execute.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the statement. This can be an anonymous object or a dictionary.
    /// </param>
    /// <returns>
    /// A <see cref="SqlMapper.GridReader"/> that contains the result sets returned by the statement.
    /// </returns>
    public SqlMapper.GridReader GetMultipleFromStatement(string commandText, dynamic? parameters = null)
        => Connection.GetMultipleFromStatement(
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
    /// Asynchronously executes a database statement (SQL command) that returns multiple result sets.
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
    /// A <see cref="SqlMapper.GridReader"/> that contains the result sets returned by the statement.
    /// </returns>
    public Task<SqlMapper.GridReader> GetMultipleFromStatementAsync(string commandText, dynamic? parameters = null, CancellationToken cancellationToken = default)
        => Connection.GetMultipleFromStatementAsync(
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