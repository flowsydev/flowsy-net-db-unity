using System.Data;
using Dapper;
using Flowsy.Db.Unity.Conventions;

namespace Flowsy.Db.Unity.Extensions;

public static partial class DbConnectionExtensions
{
    /// <summary>
    /// Executes a database statement (SQL command) asynchronously and returns the result as an enumerable collection of type T.
    /// </summary>
    /// <param name="connection">
    /// The database connection to use for executing the statement.
    /// </param>
    /// <param name="commandText">
    /// The SQL command text to execute.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the command. This can be an anonymous object or a dictionary.
    /// </param>
    /// <param name="transaction">
    /// An optional transaction to use for the command execution.
    /// </param>
    /// <param name="conventions">
    /// The conventions to use for the command execution. If null, the default conventions will be used.
    /// </param>
    /// <param name="onExecuting">
    /// An optional action to execute before the command is executed. This can be used from services to raise events or log the command.
    /// </param>
    /// <param name="onExecuted">
    /// An optional action to execute after the command is executed. This can be used from services to raise events or log the command.
    /// </param>
    /// <typeparam name="T">
    /// The type to map the results to.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of type T containing the result set returned by the command.
    /// </returns>
    public static IEnumerable<T> GetFromStatement<T>(
        this IDbConnection connection,
        string commandText,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionHandler<IEnumerable<T>>? onExecuted = null
        )
    {
        var finalConventions = conventions ?? DbConventionSet.Default;
        var commandDefinition = finalConventions.Commands.BuildDefinition(commandText, parameters as object, transaction);
        
        onExecuting?.Invoke(commandDefinition);
        var result = connection.Query<T>(commandDefinition);
        return onExecuted is not null ? onExecuted(commandDefinition, result) : result;
    }
    
    /// <summary>
    /// Asynchronously executes a database statement (SQL command) asynchronously and returns the result as an enumerable collection of type T.
    /// </summary>
    /// <param name="connection">
    /// The database connection to use for executing the statement.
    /// </param>
    /// <param name="commandText">
    /// The SQL command text to execute.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the command. This can be an anonymous object or a dictionary.
    /// </param>
    /// <param name="transaction">
    /// An optional transaction to use for the command execution.
    /// </param>
    /// <param name="conventions">
    /// The conventions to use for the command execution. If null, the default conventions will be used.
    /// </param>
    /// <param name="onExecuting">
    /// An optional action to execute before the command is executed. This can be used from services to raise events or log the command.
    /// </param>
    /// <param name="onExecuted">
    /// An optional action to execute after the command is executed. This can be used from services to raise events or log the command.
    /// </param>
    /// <param name="cancellationToken">
    /// An optional cancellation token for the asynchronous operation.
    /// </param>
    /// <typeparam name="T">
    /// The type to map the results to.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of type T containing the result set returned by the command.
    /// </returns>
    public static async Task<IEnumerable<T>> GetFromStatementAsync<T>(
        this IDbConnection connection,
        string commandText,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionAsyncHandler<IEnumerable<T>>? onExecuted = null,
        CancellationToken cancellationToken = default
        )
    {
        var finalConventions = conventions ?? DbConventionSet.Default;
        var commandDefinition = finalConventions.Commands.BuildDefinition(commandText, parameters as object, transaction, cancellationToken);
        
        onExecuting?.Invoke(commandDefinition);
        var result = await connection.QueryAsync<T>(commandDefinition);
        return onExecuted is not null ? await onExecuted(commandDefinition, result) : result;
    }
}