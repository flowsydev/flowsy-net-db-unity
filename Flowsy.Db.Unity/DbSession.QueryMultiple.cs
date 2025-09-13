using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;

namespace Flowsy.Db.Unity;

public partial class DbSession
{
    /// <summary>
    /// Performs a query to the database that can return multiple result sets.
    /// </summary>
    /// <param name="statement">
    /// SQL statement to execute that can contain multiple queries.
    /// </param>
    /// <param name="parameters">
    /// Optional parameters for the query, which can be used to prevent SQL injection.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous database query operation.
    /// The result is a GridReader object that allows reading multiple result sets.
    /// </returns>
    public async Task<SqlMapper.GridReader> QueryMultipleAsync(
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
            "[ SESSION:{SessionId} > OP:{OperationId} ] Executing QueryMultiple statement: {StatementText}", 
            SessionId,
            operationId,
            commandDefinition.CommandText
        );

        try
        {
            var result = await _connection.QueryMultipleAsync(commandDefinition);

            _logger?.Log(
                Configuration.LogLevel,
                "[ SESSION:{SessionId} > OP:{OperationId} ] QueryMultiple statement executed successfully",
                SessionId,
                operationId
            );

            return result;
        }
        catch (Exception exception)
        {
            _logger?.LogError(
                exception,
                "[ SESSION:{SessionId} > OP:{OperationId} ] Error executing QueryMultiple statement",
                SessionId,
                operationId
            );
            throw;
        }
    }
}
