using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;

namespace Flowsy.Db.Unity;

public partial class DbSession
{
    /// <summary>
    /// Performs a query to the database and returns the first result.
    /// Throws an exception if no results are found.
    /// </summary>
    /// <param name="statement">
    /// SQL statement to execute.
    /// </param>
    /// <param name="parameters">
    /// Optional parameters for the query, which can be used to prevent SQL injection.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <typeparam name="T">
    /// Type of the expected result from the query.
    /// </typeparam>
    /// <returns>
    /// A task that represents the asynchronous operation of querying the database.
    /// The result is the first element returned by the query.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the query returns no results.
    /// </exception>
    public async Task<T> QueryFirstAsync<T>(
        string statement,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
    )
    {
        await EnsureOpenConnectionAsync(cancellationToken);

        var commandDefinition = BuildCommandDefinition(
            statement,
            parameters as object,
            CommandType.Text,
            Configuration.Conventions.Commands,
            cancellationToken
        );

        var operationId = CreateOperationId();
        
        _logger?.Log(
            Configuration.LogLevel,
            "[ SESSION:{SessionId} > OP:{OperationId} ] Executing statement for query to get first result: {StatementText}",
            SessionId,
            operationId,
            commandDefinition.CommandText
        );

        try
        {
            var result = await _connection.QueryFirstAsync<T>(commandDefinition);

            _logger?.Log(
                Configuration.LogLevel,
                "[ SESSION:{SessionId} > OP:{OperationId} ] Statement executed successfully for query to get first result: {StatementText}",
                SessionId,
                operationId,
                commandDefinition.CommandText
            );

            return result;
        }
        catch (Exception exception)
        {
            _logger?.LogError(
                exception,
                "[ SESSION:{SessionId} > OP:{OperationId} ] Error executing statement for query to get first result: {StatementText}",
                SessionId,
                operationId,
                commandDefinition.CommandText
            );
            throw;
        }
    }

    /// <summary>
    /// Performs a query to the database and returns the first result, or the default value if no results are found.
    /// </summary>
    /// <param name="statement">
    /// SQL statement to execute.
    /// </param>
    /// <param name="parameters">
    /// Optional parameters for the query, which can be used to prevent SQL injection.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <typeparam name="T">
    /// Type of the expected result from the query.
    /// </typeparam>
    /// <returns>
    /// A task that represents the asynchronous operation of querying the database.
    /// The result is the first element returned by the query, or the default value of type T if no results are found.
    /// </returns>
    public async Task<T?> QueryFirstOrDefaultAsync<T>(
        string statement,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
    )
    {
        await EnsureOpenConnectionAsync(cancellationToken);

        var commandDefinition = BuildCommandDefinition(
            statement,
            parameters as object,
            CommandType.Text,
            Configuration.Conventions.Commands,
            cancellationToken
        );

        var operationId = CreateOperationId();
        
        _logger?.Log(
            Configuration.LogLevel,
            "[ SESSION:{SessionId} > OP:{OperationId} ] Executing statement for query to get first result or default: {StatementText}",
            SessionId,
            operationId,
            commandDefinition.CommandText
        );

        try
        {
            var result = await _connection.QueryFirstOrDefaultAsync<T>(commandDefinition);

            _logger?.Log(
                Configuration.LogLevel,
                "[ SESSION:{SessionId} > OP:{OperationId} ] Statement executed successfully for query to get first result or default: {StatementText}",
                SessionId,
                operationId,
                commandDefinition.CommandText
            );

            return result;
        }
        catch (Exception exception)
        {
            _logger?.LogError(
                exception,
                "[ SESSION:{SessionId} > OP:{OperationId} ] Error executing statement for query to get first result or default: {StatementText}",
                SessionId,
                operationId,
                commandDefinition.CommandText
            );
            throw;
        }
    }
}
