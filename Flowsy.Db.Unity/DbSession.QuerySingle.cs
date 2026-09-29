using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;

namespace Flowsy.Db.Unity;

public partial class DbSession
{
    /// <summary>
    /// Performs a query to the database and returns exactly one result.
    /// Throws an exception if no results are found or if multiple results are found.
    /// </summary>
    /// <param name="statement">
    /// SQL statement to execute.
    /// </param>
    /// <param name="parameters">
    /// Optional parameters for the query, which can be used to prevent SQL injection.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token for the asynchronous operation.
    /// </param>
    /// <typeparam name="T">
    /// The type of the expected result from the query.
    /// </typeparam>
    /// <returns>
    /// A task that represents the asynchronous operation of querying the database.
    /// The result is exactly one element returned by the query.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the query returns no results or multiple results.
    /// </exception>
    public async Task<T> QuerySingleAsync<T>(
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
            "[ SESSION:{SessionId} > OP:{OperationId} ] Executing statement for query to get a single result: {StatementText}",
            SessionId,
            operationId,
            commandDefinition.CommandText
        );

        try
        {
            var result = await ObserveOperationAsync("query_single", null,
                () => _connection.QuerySingleAsync<T>(commandDefinition));

            _logger?.Log(
                Configuration.LogLevel,
                "[ SESSION:{SessionId} > OP:{OperationId} ] Statement executed successfully for query to get a single result: {StatementText}",
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
                "[ SESSION:{SessionId} > OP:{OperationId} ] Error executing statement for query to get a single result: {StatementText}",
                SessionId,
                operationId,
                commandDefinition.CommandText
            );
            throw;
        }
    }

    /// <summary>
    /// Performs a query to the database and returns exactly one result, or the default value if no results are found.
    /// Throws an exception if multiple results are found.
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
    /// The result is exactly one element returned by the query, or the default value of type T if no results are found.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the query returns multiple results.
    /// </exception>
    public async Task<T?> QuerySingleOrDefaultAsync<T>(
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
            "[ SESSION:{SessionId} > OP:{OperationId} ] Executing statement to get a single result or default: {StatementText}",
            SessionId,
            operationId,
            commandDefinition.CommandText
        );

        try
        {
            var result = await ObserveOperationAsync("query_single_or_default", null,
                () => _connection.QuerySingleOrDefaultAsync<T>(commandDefinition));

            _logger?.Log(
                Configuration.LogLevel,
                "[ SESSION:{SessionId} > OP:{OperationId} ] Statement executed successfully to get a single result or default: {StatementText}",
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
                "[ SESSION:{SessionId} > OP:{OperationId} ] Error executing statement to get a single result or default: {StatementText}",
                SessionId,
                operationId,
                commandDefinition.CommandText
            );
            throw;
        }
    }
}
