using System.Data;
using Dapper;
using Flowsy.Db.Unity.Conventions;

namespace Flowsy.Db.Unity.Extensions;

public static partial class DbConnectionExtensions
{
    public static SqlMapper.GridReader GetMultipleFromRoutine(
        this IDbConnection connection,
        string routineName,
        dynamic? parameters = null, 
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionHandler<SqlMapper.GridReader>? onExecuted = null
        )
        => connection.GetMultipleFromRoutine(
            routineName,
            null,
            parameters as object,
            transaction,
            conventions,
            onExecuting,
            onExecuted
            );
    
    public static SqlMapper.GridReader GetMultipleFromRoutine(
        this IDbConnection connection,
        string routineName,
        DbRoutineType? routineType = null,
        dynamic? parameters = null, 
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionHandler<SqlMapper.GridReader>? onExecuted = null
        )
    {
        var commandDefinition = BuildCommandDefinition(routineName, routineType, parameters as object, true, transaction, conventions);
        
        onExecuting?.Invoke(commandDefinition);
        var result = connection.QueryMultiple(commandDefinition);
        return onExecuted is not null ? onExecuted(commandDefinition, result) : result;
    }
    
    public static Task<SqlMapper.GridReader> GetMultipleFromRoutineAsync(
        this IDbConnection connection,
        string routineName,
        dynamic? parameters = null, 
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionAsyncHandler<SqlMapper.GridReader>? onExecuted = null,
        CancellationToken cancellationToken = default
        )
        => connection.GetMultipleFromRoutineAsync(
            routineName,
            null,
            parameters as object,
            transaction,
            conventions,
            onExecuting,
            onExecuted,
            cancellationToken
            );
    
    public static async Task<SqlMapper.GridReader> GetMultipleFromRoutineAsync(
        this IDbConnection connection,
        string routineName,
        DbRoutineType? routineType = null,
        dynamic? parameters = null, 
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionAsyncHandler<SqlMapper.GridReader>? onExecuted = null,
        CancellationToken cancellationToken = default
        )
    {
        var commandDefinition = BuildCommandDefinition(routineName, routineType, parameters as object, true, transaction, conventions, cancellationToken);
        
        onExecuting?.Invoke(commandDefinition);
        var result = await connection.QueryMultipleAsync(commandDefinition);
        return onExecuted is not null ? await onExecuted(commandDefinition, result) : result;
    }
}