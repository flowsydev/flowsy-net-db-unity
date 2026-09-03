using Dapper;
using Microsoft.Extensions.Logging;

namespace Flowsy.Db.Unity;

public partial class DbSession
{
    /// <inheritdoc />
    public Task QueryMultipleFromRoutineAsync(
        string routineName,
        DbRoutineType routineType,
        Func<SqlMapper.GridReader, CancellationToken, Task> read,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default)
        => QueryMultipleFromRoutineAsync<object?>(routineName, routineType, async (reader, token) =>
        {
            await read(reader, token);
            return null;
        }, parameters as object, cancellationToken);

    /// <inheritdoc />
    public async Task<TResult> QueryMultipleFromRoutineAsync<TResult>(
        string routineName,
        DbRoutineType routineType,
        Func<SqlMapper.GridReader, CancellationToken, Task<TResult>> read,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(read);
        using var reader = await QueryMultipleFromRoutineAsync(routineName, routineType, parameters as object, cancellationToken);
        return await ObserveOperationAsync(
            "consume_query_multiple_routine", routineName,
            () => read(reader, cancellationToken),
            countCommand: false);
    }

    /// <summary>
    /// Performs a query to a stored procedure or function in the database that can return multiple result sets.
    /// </summary>
    /// <param name="routineName">
    /// Name of the stored procedure or function to execute.
    /// </param>
    /// <param name="parameters">
    /// Optional parameters for the routine.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous database query operation.
    /// The result is a GridReader object that allows reading multiple result sets.
    /// </returns>
    public Task<SqlMapper.GridReader> QueryMultipleFromRoutineAsync(
        string routineName,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
    )
        => QueryMultipleFromRoutineAsync(routineName, Configuration.Conventions.Routines.RoutineType, parameters as object, cancellationToken);

    /// <summary>
    /// Performs a query to a stored procedure or function in the database that can return multiple result sets.
    /// </summary>
    /// <param name="routineName">
    /// Name of the stored procedure or function to execute.
    /// </param>
    /// <param name="routineType">
    /// Type of routine (stored procedure or function).
    /// </param>
    /// <param name="parameters">
    /// Optional parameters for the routine.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous database query operation.
    /// The result is a GridReader object that allows reading multiple result sets.
    /// </returns>
    public async Task<SqlMapper.GridReader> QueryMultipleFromRoutineAsync(
        string routineName,
        DbRoutineType routineType,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
    )
    {
        await EnsureOpenConnectionAsync(cancellationToken);

        var (routineConvention, commandConvention) = Configuration.Conventions;

        var commandDefinition = BuildCommandDefinition(
            routineName,
            routineType,
            parameters as object,
            true,
            routineConvention,
            commandConvention,
            cancellationToken
        );

        var operationId = CreateOperationId();
        
        _logger?.Log(
            Configuration.LogLevel,
            "[ SESSION:{SessionId} > OP:{OperationId} ] Executing routine for query with multiple result sets: {RoutineCall}",
            SessionId,
            operationId,
            commandDefinition.CommandText
        );

        try
        {
            var result = await ObserveOperationAsync("query_multiple_routine", routineName,
                () => _connection.QueryMultipleAsync(commandDefinition));

            _logger?.Log(
                Configuration.LogLevel,
                "[ SESSION:{SessionId} > OP:{OperationId} ] Routine executed successfully for query with multiple result sets: {RoutineCall}",
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
                "[ SESSION:{SessionId} > OP:{OperationId} ] Error executing routine for query with multiple result sets: {RoutineCall}",
                SessionId,
                operationId,
                commandDefinition.CommandText
            );
            throw;
        }
    }
}
