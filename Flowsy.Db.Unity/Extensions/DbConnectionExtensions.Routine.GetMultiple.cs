using System.Data;
using Dapper;
using Flowsy.Db.Unity.Conventions;

namespace Flowsy.Db.Unity.Extensions;

public static partial class DbConnectionExtensions
{
    /// <summary>
    /// Executes a database routine (stored procedure or function) that returns multiple result sets.
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
    /// <returns>
    /// A <see cref="SqlMapper.GridReader"/> that contains the result sets returned by the routine.
    /// </returns>
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
    
    /// <summary>
    /// Executes a database routine (stored procedure or function) that returns multiple result sets.
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
    /// <returns>
    /// A <see cref="SqlMapper.GridReader"/> that contains the result sets returned by the routine.
    /// </returns>
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

    /// <summary>
    /// Asynchronously executes a database routine (stored procedure or function) that returns multiple result sets.
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
    /// <returns>
    /// A <see cref="SqlMapper.GridReader"/> that contains the result sets returned by the routine.
    /// </returns>
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

    /// <summary>
    /// Asynchronously executes a database routine (stored procedure or function) that returns multiple result sets.
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
    /// <returns>
    /// A <see cref="SqlMapper.GridReader"/> that contains the result sets returned by the routine.
    /// </returns>
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