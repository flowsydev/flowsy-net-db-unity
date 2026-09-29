using Dapper;
using Microsoft.Extensions.Logging;

namespace Flowsy.Db.Unity;

public partial class DbSession
{
    /// <summary>
    /// Executes a database routine (stored procedure or function) that does not return results.
    /// </summary>
    /// <param name="routineName">
    /// Name of the routine to execute.
    /// </param>
    /// <param name="parameters">
    /// Optional parameters for the routine, which can be used to prevent SQL injection.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of executing the routine.
    /// The result is the number of rows affected by the execution of the routine.
    /// </returns>
    public Task<int> ExecuteRoutineAsync(
        string routineName,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
        )
        => ExecuteRoutineAsync(routineName, Configuration.Conventions.Routines.RoutineType, parameters as object, cancellationToken);
    
    /// <summary>
    /// Executes a database routine (stored procedure or function) that does not return results.
    /// </summary>
    /// <param name="routineName">
    /// Name of the routine to execute.
    /// </param>
    /// <param name="routineType">
    /// Type of the routine (stored procedure or function).
    /// </param>
    /// <param name="parameters">
    /// Optional parameters for the routine, which can be used to prevent SQL injection.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of executing the routine.
    /// The result is the number of rows affected by the execution of the routine.
    /// </returns>
    public async Task<int> ExecuteRoutineAsync(
        string routineName,
        DbRoutineType routineType,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
    )
    {
        EnsureRoutineWriteAllowed(routineName);
        await EnsureOpenConnectionAsync(cancellationToken);

        var (routineConvention, commandConvention) = Configuration.Conventions;
        
        var commandDefinition = BuildCommandDefinition(
            routineName,
            routineType,
            parameters as object,
            false,
            routineConvention,
            commandConvention,
            cancellationToken
        );

        var operationId = CreateOperationId();
        
        _logger?.Log(
            Configuration.LogLevel,
            "[ SESSION:{SessionId} > OP:{OperationId} ] Executing routine: {RoutineCall}", 
            SessionId,
            operationId,
            commandDefinition.CommandText
            );

        try
        {
            var result = await ObserveOperationAsync("execute_routine", routineName,
                () => _connection.ExecuteAsync(commandDefinition));

            _logger?.Log(
                Configuration.LogLevel,
                "[ SESSION:{SessionId} > OP:{OperationId} ] Routine executed affecting {RowsAffected}: {RoutineCall}",
                SessionId,
                operationId,
                result,
                commandDefinition.CommandText
            );
            
            return result;
        }
        catch (Exception exception)
        {
            _logger?.LogError(
                exception,
                "[ SESSION:{SessionId} > OP:{OperationId} ] Error executing routine: {RoutineCall}",
                SessionId,
                operationId,
                commandDefinition.CommandText
            );
            throw;
        }
    }
}
