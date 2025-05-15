using System.Data;
using Dapper;
using Flowsy.Db.Unity.Conventions;

namespace Flowsy.Db.Unity.Extensions;

public static partial class DbConnectionExtensions
{
    /// <summary>
    /// Asynchronously executes a database statement (SQL command) and returns a single result as an instance of T.
    /// If a primitive type is required (int, string, etc.), then the first column of the single result found will be returned.
    /// If no results are found or more than one result is found, an exception will be thrown.
    /// </summary>
    /// <param name="connection">
    /// The database connection to use for executing the statement.
    /// </param>
    /// <param name="commandText">
    /// The text of the SQL statement to execute.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the statement. This can be an anonymous object or a dictionary.
    /// </param>
    /// <param name="transaction">
    /// An optional transaction to use for the statement execution.
    /// </param>
    /// <param name="conventions">
    /// The conventions to use for the statement execution. If null, the default conventions will be used.
    /// </param>
    /// <param name="onExecuting">
    /// An optional action to execute before the statement is executed. This can be used from services to raise events or log the command.
    /// </param>
    /// <param name="onExecuted">
    /// An optional action to execute after the statement is executed. This can be used from services to raise events or log the command.
    /// </param>
    /// <typeparam name="T">
    /// The type of the result to return. This can be a primitive type or a complex type.
    /// </typeparam>
    /// <returns>
    /// The single result of the statement execution as an instance of T.
    /// </returns>
    public static T GetSingleFromStatement<T>(
        this IDbConnection connection,
        string commandText,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionHandler<T>? onExecuted = null
        )
    {
        var finalConventions = conventions ?? DbConventionSet.Default;
        var commandDefinition = finalConventions.Commands.BuildDefinition(commandText, parameters as object, transaction);
        
        onExecuting?.Invoke(commandDefinition);
        var result = connection.QuerySingle<T>(commandDefinition);
        return onExecuted is not null ? onExecuted(commandDefinition, result) : result;
    }

    /// <summary>
    /// Asynchronously executes a database statement (SQL command) and returns a single result as an instance of T.
    /// If a primitive type is required (int, string, etc.), then the first column of the single result found will be returned.
    /// If no results are found or more than one result is found, an exception will be thrown.
    /// </summary>
    /// <param name="connection">
    /// The database connection to use for executing the statement.
    /// </param>
    /// <param name="commandText">
    /// The text of the SQL statement to execute.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the statement. This can be an anonymous object or a dictionary.
    /// </param>
    /// <param name="transaction">
    /// An optional transaction to use for the statement execution.
    /// </param>
    /// <param name="conventions">
    /// The conventions to use for the statement execution. If null, the default conventions will be used.
    /// </param>
    /// <param name="onExecuting">
    /// An optional action to execute before the statement is executed. This can be used from services to raise events or log the command.
    /// </param>
    /// <param name="onExecuted">
    /// An optional action to execute after the statement is executed. This can be used from services to raise events or log the command.
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
    public static async Task<T> GetSingleFromStatementAsync<T>(
        this IDbConnection connection,
        string commandText,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionAsyncHandler<T>? onExecuted = null,
        CancellationToken cancellationToken = default
        )
    {
        var finalConventions = conventions ?? DbConventionSet.Default;
        var commandDefinition = finalConventions.Commands.BuildDefinition(commandText, parameters as object, transaction, cancellationToken);
        
        onExecuting?.Invoke(commandDefinition);
        var result = await connection.QuerySingleAsync<T>(commandDefinition);
        return onExecuted is not null ? await onExecuted(commandDefinition, result) : result;
    }
}