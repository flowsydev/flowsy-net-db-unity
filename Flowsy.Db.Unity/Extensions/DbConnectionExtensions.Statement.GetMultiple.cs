using System.Data;
using Dapper;
using Flowsy.Db.Unity.Conventions;

namespace Flowsy.Db.Unity.Extensions;

public static partial class DbConnectionExtensions
{   
    /// <summary>
    /// Executes a database statement (SQL command) that returns multiple result sets.
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
    /// <returns>
    /// A <see cref="SqlMapper.GridReader"/> that contains the result sets returned by the statement.
    /// </returns>
    public static SqlMapper.GridReader GetMultipleFromStatement(
        this IDbConnection connection,
        string commandText,
        dynamic? parameters = null, 
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionHandler<SqlMapper.GridReader>? onExecuted = null
        )
    {
        var finalConventions = conventions ?? DbConventionSet.Default;
        var commandDefinition = finalConventions.Commands.BuildDefinition(commandText, parameters as object, transaction);
        
        onExecuting?.Invoke(commandDefinition);
        var result = connection.QueryMultiple(commandDefinition);
        return onExecuted is not null ? onExecuted(commandDefinition, result) : result;
    }

    /// <summary>
    /// Asynchronously executes a database statement (SQL command) that returns multiple result sets.
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
    /// <returns>
    /// A <see cref="SqlMapper.GridReader"/> that contains the result sets returned by the statement.
    /// </returns>
    public static async Task<SqlMapper.GridReader> GetMultipleFromStatementAsync(
        this IDbConnection connection,
        string commandText,
        dynamic? parameters = null, 
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionAsyncHandler<SqlMapper.GridReader>? onExecuted = null,
        CancellationToken cancellationToken = default
        )
    {
        var finalConventions = conventions ?? DbConventionSet.Default;
        var commandDefinition = finalConventions.Commands.BuildDefinition(commandText, parameters as object, transaction, cancellationToken);
        
        onExecuting?.Invoke(commandDefinition);
        var result = await connection.QueryMultipleAsync(commandDefinition);
        return onExecuted is not null ? await onExecuted(commandDefinition, result) : result;
    }
}