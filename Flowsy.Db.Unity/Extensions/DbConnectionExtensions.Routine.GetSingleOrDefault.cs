using System.Data;
using Dapper;
using Flowsy.Db.Unity.Conventions;

namespace Flowsy.Db.Unity.Extensions;

public static partial class DbConnectionExtensions
{
    /// <summary>
    /// Executes a database routine (stored procedure or function) and returns a single result as an instance of T.
    /// If a primitive type is required (int, string, etc.), then the first column of the single result found will be returned.
    /// If no results are found, the default value of T will be returned.
    /// If multiple results are found, an exception will be thrown.
    /// </summary>
    /// <param name="connection">
    /// The database connection to use for executing the routine.
    /// </param>
    /// <param name="routineName">
    /// The name of the routine to execute.
    /// The type of the routine (stored procedure or function) will be resolved from the provided conventions.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the routine. This can be an anonymous object or a dictionary.
    /// </param>
    /// <param name="transaction">
    /// An optional transaction to use for the routine execution.
    /// </param>
    /// <param name="conventions">
    /// The conventions to use for the routine execution. If null, the default conventions will be used.
    /// </param>
    /// <param name="onExecuting">
    /// An optional action to execute before the routine is executed. This can be used from services to raise events or log the command.
    /// </param>
    /// <param name="onExecuted">
    /// An optional action to execute after the routine is executed. This can be used from services to raise events or log the command.
    /// </param>
    /// <typeparam name="T">
    /// The type of the result to return. This can be a primitive type or a complex type.
    /// </typeparam>
    /// <returns>
    /// The single result of the routine execution as an instance of T.
    /// </returns>
    public static T? GetSingleOrDefaultFromRoutine<T>(
        this IDbConnection connection,
        string routineName, 
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionHandler<T?>? onExecuted = null
        )
        => connection.GetSingleOrDefaultFromRoutine(
            routineName,
            null,
            parameters as object,
            transaction,
            conventions,
            onExecuting,
            onExecuted
            );
        
    /// <summary>
    /// Executes a database routine (stored procedure or function) and returns a single result as an instance of T.
    /// If a primitive type is required (int, string, etc.), then the first column of the single result found will be returned.
    /// If no results are found, the default value of T will be returned.
    /// If multiple results are found, an exception will be thrown.
    /// </summary>
    /// <param name="connection">
    /// The database connection to use for executing the routine.
    /// </param>
    /// <param name="routineName">
    /// The name of the routine to execute.
    /// </param>
    /// <param name="routineType">
    /// The type of the routine (stored procedure or function). If null, the type will be resolved from the provided conventions.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the routine. This can be an anonymous object or a dictionary.
    /// </param>
    /// <param name="transaction">
    /// An optional transaction to use for the routine execution.
    /// </param>
    /// <param name="conventions">
    /// The conventions to use for the routine execution. If null, the default conventions will be used.
    /// </param>
    /// <param name="onExecuting">
    /// An optional action to execute before the routine is executed. This can be used from services to raise events or log the command.
    /// </param>
    /// <param name="onExecuted">
    /// An optional action to execute after the routine is executed. This can be used from services to raise events or log the command.
    /// </param>
    /// <typeparam name="T">
    /// The type of the result to return. This can be a primitive type or a complex type.
    /// </typeparam>
    /// <returns>
    /// The single result of the routine execution as an instance of T.
    /// </returns>
    public static T? GetSingleOrDefaultFromRoutine<T>(
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
        var result = connection.QuerySingleOrDefault<T>(commandDefinition);
        return onExecuted is not null ? onExecuted.Invoke(commandDefinition, result) : result;
    }
    
    /// <summary>
    /// Executes a database routine (stored procedure or function) and returns a single result as an instance of T.
    /// If a primitive type is required (int, string, etc.), then the first column of the single result found will be returned.
    /// If no results are found, the default value of T will be returned.
    /// If multiple results are found, an exception will be thrown.
    /// </summary>
    /// <param name="connection">
    /// The database connection to use for executing the routine.
    /// </param>
    /// <param name="routineName">
    /// The name of the routine to execute.
    /// The type of the routine (stored procedure or function) will be resolved from the provided conventions.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the routine. This can be an anonymous object or a dictionary.
    /// </param>
    /// <param name="transaction">
    /// An optional transaction to use for the routine execution.
    /// </param>
    /// <param name="conventions">
    /// The conventions to use for the routine execution. If null, the default conventions will be used.
    /// </param>
    /// <param name="onExecuting">
    /// An optional action to execute before the routine is executed. This can be used from services to raise events or log the command.
    /// </param>
    /// <param name="onExecuted">
    /// An optional action to execute after the routine is executed. This can be used from services to raise events or log the command.
    /// </param>
    /// <param name="cancellationToken">
    /// An optional cancellation token for the asynchronous operation.
    /// </param>
    /// <typeparam name="T">
    /// The type of the result to return. This can be a primitive type or a complex type.
    /// </typeparam>
    /// <returns>
    /// The single result of the routine execution as an instance of T.
    /// </returns>
    public static Task<T?> GetSingleOrDefaultFromRoutineAsync<T>(
        this IDbConnection connection,
        string routineName, 
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionAsyncHandler<T?>? onExecuted = null,
        CancellationToken cancellationToken = default
        )
        => connection.GetSingleOrDefaultFromRoutineAsync(
            routineName,
            null,
            parameters as object,
            transaction,
            conventions,
            onExecuting,
            onExecuted,
            cancellationToken
            );
        
    /// <summary>
    /// Asynchronously executes a database routine (stored procedure or function) and returns a single result as an instance of T.
    /// If a primitive type is required (int, string, etc.), then the first column of the single result found will be returned.
    /// If no results are found, the default value of T will be returned.
    /// If multiple results are found, an exception will be thrown.
    /// </summary>
    /// <param name="connection">
    /// The database connection to use for executing the routine.
    /// </param>
    /// <param name="routineName">
    /// The name of the routine to execute.
    /// </param>
    /// <param name="routineType">
    /// The type of the routine (stored procedure or function). If null, the type will be resolved from the provided conventions.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the routine. This can be an anonymous object or a dictionary.
    /// </param>
    /// <param name="transaction">
    /// An optional transaction to use for the routine execution.
    /// </param>
    /// <param name="conventions">
    /// The conventions to use for the routine execution. If null, the default conventions will be used.
    /// </param>
    /// <param name="onExecuting">
    /// An optional action to execute before the routine is executed. This can be used from services to raise events or log the command.
    /// </param>
    /// <param name="onExecuted">
    /// An optional action to execute after the routine is executed. This can be used from services to raise events or log the command.
    /// </param>
    /// <param name="cancellationToken">
    /// An optional cancellation token for the asynchronous operation.
    /// </param>
    /// <typeparam name="T">
    /// The type of the result to return. This can be a primitive type or a complex type.
    /// </typeparam>
    /// <returns>
    /// The single result of the routine execution as an instance of T.
    /// </returns>
    public static async Task<T?> GetSingleOrDefaultFromRoutineAsync<T>(
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
        var result = await connection.QuerySingleOrDefaultAsync<T?>(commandDefinition);
        return onExecuted is not null ? await onExecuted(commandDefinition, result) : result;
    }
}