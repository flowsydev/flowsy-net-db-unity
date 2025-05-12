using System.Data;
using Dapper;
using Flowsy.Db.Unity.Conventions;

namespace Flowsy.Db.Unity.Extensions;

public static partial class DbConnectionExtensions
{
    public static int ExecuteRoutine(
        this IDbConnection connection,
        string routineName,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionHandler<int>? onExecuted = null
        )
        => connection.ExecuteRoutine(routineName, null, parameters as object, transaction, conventions, onExecuting, onExecuted); 
    
    public static int ExecuteRoutine(
        this IDbConnection connection,
        string routineName,
        DbRoutineType? routineType = null,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionHandler<int>? onExecuted = null
        )
    {
        var commandDefinition = BuildCommandDefinition(routineName, routineType, parameters as object, true, transaction, conventions);
        
        onExecuting?.Invoke(commandDefinition);
        var result = connection.Execute(commandDefinition);
        return onExecuted?.Invoke(commandDefinition, result) ?? result;
    }
    
    public static Task<int> ExecuteRoutineAsync(
        this IDbConnection connection,
        string routineName,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionAsyncHandler<int>? onExecuted = null,
        CancellationToken cancellationToken = default
        )
        => connection.ExecuteRoutineAsync(routineName, null, parameters as object, transaction, conventions, onExecuting, onExecuted, cancellationToken); 
    
    public static async Task<int> ExecuteRoutineAsync(
        this IDbConnection connection,
        string routineName,
        DbRoutineType? routineType = null,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionAsyncHandler<int>? onExecuted = null,
        CancellationToken cancellationToken = default
        )
    {
        var commandDefinition = BuildCommandDefinition(routineName, routineType, parameters as object, true, transaction, conventions, cancellationToken);
        
        onExecuting?.Invoke(commandDefinition);
        var result = await connection.ExecuteAsync(commandDefinition);
        return onExecuted is not null ? await onExecuted(commandDefinition, result) : result;
    }
}