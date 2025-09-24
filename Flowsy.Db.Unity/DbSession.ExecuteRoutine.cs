using Dapper;
using Microsoft.Extensions.Logging;

namespace Flowsy.Db.Unity;

public partial class DbSession
{
    public Task<int> ExecuteRoutineAsync(
        string routineName,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
        )
        => ExecuteRoutineAsync(routineName, Configuration.Conventions.Routines.RoutineType, parameters as object, cancellationToken);
    
    public async Task<int> ExecuteRoutineAsync(
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
            var result = await _connection.ExecuteAsync(commandDefinition);

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