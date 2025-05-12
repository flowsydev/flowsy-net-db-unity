using System.Data;
using Dapper;
using Flowsy.Db.Unity.Conventions;

namespace Flowsy.Db.Unity.Extensions;

public static partial class DbConnectionExtensions
{
    public static IEnumerable<T> GetFromRoutine<T>(
        this IDbConnection connection,
        string routineName, 
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionHandler<IEnumerable<T>>? onExecuted = null
        )
        => connection.GetFromRoutine(
            routineName,
            null,
            parameters as object,
            transaction,
            conventions,
            onExecuting,
            onExecuted
            );
        
    public static IEnumerable<T> GetFromRoutine<T>(
        this IDbConnection connection,
        string routineName,
        DbRoutineType? routineType = null,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionHandler<IEnumerable<T>>? onExecuted = null
        )
    {
        var commandDefinition = BuildCommandDefinition(routineName, routineType, parameters as object, true, transaction, conventions);
        
        onExecuting?.Invoke(commandDefinition);
        var result = connection.Query<T>(commandDefinition);
        return onExecuted is not null ? onExecuted(commandDefinition, result) : result;
    }
    
    public static Task<IEnumerable<T>> GetFromRoutineAsync<T>(
        this IDbConnection connection,
        string routineName, 
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionAsyncHandler<IEnumerable<T>>? onExecuted = null,
        CancellationToken cancellationToken = default
        )
        => connection.GetFromRoutineAsync(
            routineName,
            null,
            parameters as object,
            transaction,
            conventions,
            onExecuting,
            onExecuted,
            cancellationToken
            );
        
    public async static Task<IEnumerable<T>> GetFromRoutineAsync<T>(
        this IDbConnection connection,
        string routineName,
        DbRoutineType? routineType = null,
        dynamic? parameters = null, 
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionAsyncHandler<IEnumerable<T>>? onExecuted = null,
        CancellationToken cancellationToken = default
        )
    {
        var commandDefinition = BuildCommandDefinition(routineName, routineType, parameters as object, true, transaction, conventions, cancellationToken);
        
        onExecuting?.Invoke(commandDefinition);
        var result = await connection.QueryAsync<T>(commandDefinition);
        return onExecuted is not null ? await onExecuted(commandDefinition, result) : result;
    }
}