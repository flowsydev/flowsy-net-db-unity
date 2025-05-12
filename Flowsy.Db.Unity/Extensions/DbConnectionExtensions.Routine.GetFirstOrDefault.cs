using System.Data;
using Dapper;
using Flowsy.Db.Unity.Conventions;

namespace Flowsy.Db.Unity.Extensions;

public static partial class DbConnectionExtensions
{
    public static T? GetFirstOrDefaultFromRoutine<T>(
        this IDbConnection connection,
        string routineName, 
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionHandler<T?>? onExecuted = null
        )
        => connection.GetFirstOrDefaultFromRoutine(
            routineName,
            null,
            parameters as object,
            transaction,
            conventions,
            onExecuting,
            onExecuted
            );
        
    public static T? GetFirstOrDefaultFromRoutine<T>(
        this IDbConnection connection,
        string routineName,
        DbRoutineType? routineType = null,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionHandler<T?>? onExecuted = null
        )
    {
        var commandDefinition = BuildCommandDefinition(routineName, routineType, parameters as object, true, transaction, conventions);
        
        onExecuting?.Invoke(commandDefinition);
        var result = connection.QueryFirstOrDefault<T>(commandDefinition);
        return onExecuted is not null ? onExecuted.Invoke(commandDefinition, result) : result;
    }
    
    public static Task<T?> GetFirstOrDefaultFromRoutineAsync<T>(
        this IDbConnection connection,
        string routineName, 
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionAsyncHandler<T?>? onExecuted = null,
        CancellationToken cancellationToken = default
        )
        => connection.GetFirstOrDefaultFromRoutineAsync(
            routineName,
            null,
            parameters as object,
            transaction,
            conventions,
            onExecuting,
            onExecuted,
            cancellationToken
            );
        
    public async static Task<T?> GetFirstOrDefaultFromRoutineAsync<T>(
        this IDbConnection connection,
        string routineName,
        DbRoutineType? routineType = null,
        dynamic? parameters = null, 
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionAsyncHandler<T?>? onExecuted = null,
        CancellationToken cancellationToken = default
        )
    {
        var commandDefinition = BuildCommandDefinition(routineName, routineType, parameters as object, true, transaction, conventions, cancellationToken);
        
        onExecuting?.Invoke(commandDefinition);
        var result = await connection.QueryFirstAsync<T?>(commandDefinition);
        return onExecuted is not null ? await onExecuted(commandDefinition, result) : result;
    }
}