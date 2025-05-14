using System.Data;
using Dapper;
using Flowsy.Db.Unity.Conventions;

namespace Flowsy.Db.Unity.Extensions;

public static partial class DbConnectionExtensions
{
    public static IEnumerable<TReturn> GetFromRoutine<TFirst, TSecond, TReturn>(
        this IDbConnection connection,
        string routineName,
        string splitOn,
        Func<TFirst, TSecond, TReturn> map,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionHandler<IEnumerable<TReturn>>? onExecuted = null
        )
        => connection.GetFromRoutine(
            routineName,
            null,
            splitOn,
            map,
            parameters as object,
            transaction,
            conventions,
            onExecuting,
            onExecuted
            );
    
    public static IEnumerable<TReturn> GetFromRoutine<TFirst, TSecond, TReturn>(
        this IDbConnection connection,
        string routineName,
        DbRoutineType? routineType,
        string splitOn,
        Func<TFirst, TSecond, TReturn> map,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionHandler<IEnumerable<TReturn>>? onExecuted = null
    )
    {
        var commandDefinition = BuildCommandDefinition(routineName, routineType, parameters as object, true, transaction, conventions);
        
        onExecuting?.Invoke(commandDefinition);
        var result = connection.Query(
            commandDefinition.CommandText,
            map,
            commandDefinition.Parameters,
            transaction,
            commandDefinition.Flags == CommandFlags.Buffered,
            splitOn,
            commandDefinition.CommandTimeout,
            commandDefinition.CommandType
            );
        return onExecuted is not null ? onExecuted(commandDefinition, result) : result;
    }
    
    public static IEnumerable<TReturn> GetFromRoutine<TFirst, TSecond, TThird, TReturn>(
        this IDbConnection connection,
        string routineName,
        string splitOn,
        Func<TFirst, TSecond, TThird, TReturn> map,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionHandler<IEnumerable<TReturn>>? onExecuted = null
        )
        => connection.GetFromRoutine(
            routineName,
            null,
            splitOn,
            map,
            parameters as object,
            transaction,
            conventions,
            onExecuting,
            onExecuted
            );
    
    public static IEnumerable<TReturn> GetFromRoutine<TFirst, TSecond, TThird, TReturn>(
        this IDbConnection connection,
        string routineName,
        DbRoutineType? routineType,
        string splitOn,
        Func<TFirst, TSecond, TThird, TReturn> map,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionHandler<IEnumerable<TReturn>>? onExecuted = null
    )
    {
        var commandDefinition = BuildCommandDefinition(routineName, routineType, parameters as object, true, transaction, conventions);
        
        onExecuting?.Invoke(commandDefinition);
        var result = connection.Query(
            commandDefinition.CommandText,
            map,
            commandDefinition.Parameters,
            transaction,
            commandDefinition.Flags == CommandFlags.Buffered,
            splitOn,
            commandDefinition.CommandTimeout,
            commandDefinition.CommandType
            );
        return onExecuted is not null ? onExecuted(commandDefinition, result) : result;
    }
    
    public static IEnumerable<TReturn> GetFromRoutine<TFirst, TSecond, TThird, TFourth, TReturn>(
        this IDbConnection connection,
        string routineName,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TReturn> map,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionHandler<IEnumerable<TReturn>>? onExecuted = null
        )
        => connection.GetFromRoutine(
            routineName,
            null,
            splitOn,
            map,
            parameters as object,
            transaction,
            conventions,
            onExecuting,
            onExecuted
            );
    
    public static IEnumerable<TReturn> GetFromRoutine<TFirst, TSecond, TThird, TFourth, TReturn>(
        this IDbConnection connection,
        string routineName,
        DbRoutineType? routineType,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TReturn> map,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionHandler<IEnumerable<TReturn>>? onExecuted = null
    )
    {
        var commandDefinition = BuildCommandDefinition(routineName, routineType, parameters as object, true, transaction, conventions);
        
        onExecuting?.Invoke(commandDefinition);
        var result = connection.Query(
            commandDefinition.CommandText,
            map,
            commandDefinition.Parameters,
            transaction,
            commandDefinition.Flags == CommandFlags.Buffered,
            splitOn,
            commandDefinition.CommandTimeout,
            commandDefinition.CommandType
            );
        return onExecuted is not null ? onExecuted(commandDefinition, result) : result;
    }
    
    public static IEnumerable<TReturn> GetFromRoutine<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(
        this IDbConnection connection,
        string routineName,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionHandler<IEnumerable<TReturn>>? onExecuted = null
        )
        => connection.GetFromRoutine(
            routineName,
            null,
            splitOn,
            map,
            parameters as object,
            transaction,
            conventions,
            onExecuting,
            onExecuted
            );
    
    public static IEnumerable<TReturn> GetFromRoutine<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(
        this IDbConnection connection,
        string routineName,
        DbRoutineType? routineType,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionHandler<IEnumerable<TReturn>>? onExecuted = null
    )
    {
        var commandDefinition = BuildCommandDefinition(routineName, routineType, parameters as object, true, transaction, conventions);
        
        onExecuting?.Invoke(commandDefinition);
        var result = connection.Query(
            commandDefinition.CommandText,
            map,
            commandDefinition.Parameters,
            transaction,
            commandDefinition.Flags == CommandFlags.Buffered,
            splitOn,
            commandDefinition.CommandTimeout,
            commandDefinition.CommandType
            );
        return onExecuted is not null ? onExecuted(commandDefinition, result) : result;
    }
    
    public static IEnumerable<TReturn> GetFromRoutine<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(
        this IDbConnection connection,
        string routineName,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionHandler<IEnumerable<TReturn>>? onExecuted = null
        )
        => connection.GetFromRoutine(
            routineName,
            null,
            splitOn,
            map,
            parameters as object,
            transaction,
            conventions,
            onExecuting,
            onExecuted
            );
    
    public static IEnumerable<TReturn> GetFromRoutine<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(
        this IDbConnection connection,
        string routineName,
        DbRoutineType? routineType,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionHandler<IEnumerable<TReturn>>? onExecuted = null
    )
    {
        var commandDefinition = BuildCommandDefinition(routineName, routineType, parameters as object, true, transaction, conventions);
        
        onExecuting?.Invoke(commandDefinition);
        var result = connection.Query(
            commandDefinition.CommandText,
            map,
            commandDefinition.Parameters,
            transaction,
            commandDefinition.Flags == CommandFlags.Buffered,
            splitOn,
            commandDefinition.CommandTimeout,
            commandDefinition.CommandType
            );
        return onExecuted is not null ? onExecuted(commandDefinition, result) : result;
    }
    
    public static IEnumerable<TReturn> GetFromRoutine<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(
        this IDbConnection connection,
        string routineName,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionHandler<IEnumerable<TReturn>>? onExecuted = null
        )
        => connection.GetFromRoutine(
            routineName,
            null,
            splitOn,
            map,
            parameters as object,
            transaction,
            conventions,
            onExecuting,
            onExecuted
            );
    
    /// <summary>
    /// Executes a stored procedure or function and maps the result to multiple objects.
    /// </summary>
    /// <param name="connection">
    /// The database connection.
    /// </param>
    /// <param name="routineName">
    /// The name of the stored procedure or function.
    /// </param>
    /// <param name="routineType">
    /// A value of <see cref="DbRoutineType"/> indicating the type of routine (e.g., stored procedure or function).
    /// </param>
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the stored procedure or function.
    /// </param>
    /// <param name="transaction">
    /// The database transaction to use.
    /// </param>
    /// <param name="conventions">
    /// The conventions to use for invoking the routine.
    /// </param>
    /// <param name="onExecuting">
    /// A callback function that is executed before the database command is invoked.
    /// </param>
    /// <param name="onExecuted">
    /// A callback function that is executed after the database command is invoked.
    /// </param>
    /// <typeparam name="TFirst">
    /// The type of the first object in the result set.
    /// </typeparam>
    /// <typeparam name="TSecond">
    /// The type of the second object in the result set.
    /// </typeparam>
    /// <typeparam name="TThird">
    /// The type of the third object in the result set.
    /// </typeparam>
    /// <typeparam name="TFourth">
    /// The type of the fourth object in the result set.
    /// </typeparam>
    /// <typeparam name="TFifth">
    /// The type of the fifth object in the result set.
    /// </typeparam>
    /// <typeparam name="TSixth">
    /// The type of the sixth object in the result set.
    /// </typeparam>
    /// <typeparam name="TSeventh">
    /// The type of the seventh object in the result set.
    /// </typeparam>
    /// <typeparam name="TReturn">
    /// The type of the object to return.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of the mapped objects.
    /// </returns>
    public static IEnumerable<TReturn> GetFromRoutine<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(
        this IDbConnection connection,
        string routineName,
        DbRoutineType? routineType,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionHandler<IEnumerable<TReturn>>? onExecuted = null
    )
    {
        var commandDefinition = BuildCommandDefinition(routineName, routineType, parameters as object, true, transaction, conventions);
        
        onExecuting?.Invoke(commandDefinition);
        var result = connection.Query(
            commandDefinition.CommandText,
            map,
            commandDefinition.Parameters,
            transaction,
            commandDefinition.Flags == CommandFlags.Buffered,
            splitOn,
            commandDefinition.CommandTimeout,
            commandDefinition.CommandType
            );
        return onExecuted is not null ? onExecuted(commandDefinition, result) : result;
    }
    
    public static Task<IEnumerable<TReturn>> GetFromRoutineAsync<TFirst, TSecond, TReturn>(
        this IDbConnection connection,
        string routineName,
        string splitOn,
        Func<TFirst, TSecond, TReturn> map,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionAsyncHandler<IEnumerable<TReturn>>? onExecuted = null
    ) => connection.GetFromRoutineAsync(
        routineName,
        null,
        splitOn,
        map,
        parameters as object,
        transaction,
        conventions,
        onExecuting,
        onExecuted
        );
    
    public static async Task<IEnumerable<TReturn>> GetFromRoutineAsync<TFirst, TSecond, TReturn>(
        this IDbConnection connection,
        string routineName,
        DbRoutineType? routineType,
        string splitOn,
        Func<TFirst, TSecond, TReturn> map,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionAsyncHandler<IEnumerable<TReturn>>? onExecuted = null
    )
    {
        var commandDefinition = BuildCommandDefinition(routineName, routineType, parameters as object, true, transaction, conventions);
        
        onExecuting?.Invoke(commandDefinition);
        var result = await connection.QueryAsync(
            commandDefinition.CommandText,
            map,
            commandDefinition.Parameters,
            transaction,
            commandDefinition.Flags == CommandFlags.Buffered,
            splitOn,
            commandDefinition.CommandTimeout,
            commandDefinition.CommandType
            );
        return onExecuted is not null ? await onExecuted(commandDefinition, result) : result;
    }
    
    public static Task<IEnumerable<TReturn>> GetFromRoutineAsync<TFirst, TSecond, TThird, TReturn>(
        this IDbConnection connection,
        string routineName,
        string splitOn,
        Func<TFirst, TSecond, TThird, TReturn> map,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionAsyncHandler<IEnumerable<TReturn>>? onExecuted = null
    ) => connection.GetFromRoutineAsync(
        routineName,
        null,
        splitOn,
        map,
        parameters as object,
        transaction,
        conventions,
        onExecuting,
        onExecuted
        );
    
    public static async Task<IEnumerable<TReturn>> GetFromRoutineAsync<TFirst, TSecond, TThird, TReturn>(
        this IDbConnection connection,
        string routineName,
        DbRoutineType? routineType,
        string splitOn,
        Func<TFirst, TSecond, TThird, TReturn> map,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionAsyncHandler<IEnumerable<TReturn>>? onExecuted = null
    )
    {
        var commandDefinition = BuildCommandDefinition(routineName, routineType, parameters as object, true, transaction, conventions);
        
        onExecuting?.Invoke(commandDefinition);
        var result = await connection.QueryAsync(
            commandDefinition.CommandText,
            map,
            commandDefinition.Parameters,
            transaction,
            commandDefinition.Flags == CommandFlags.Buffered,
            splitOn,
            commandDefinition.CommandTimeout,
            commandDefinition.CommandType
            );
        return onExecuted is not null ? await onExecuted(commandDefinition, result) : result;
    }
    
    public static Task<IEnumerable<TReturn>> GetFromRoutineAsync<TFirst, TSecond, TThird, TFourth, TReturn>(
        this IDbConnection connection,
        string routineName,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TReturn> map,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionAsyncHandler<IEnumerable<TReturn>>? onExecuted = null
    ) => connection.GetFromRoutineAsync(
        routineName,
        null,
        splitOn,
        map,
        parameters as object,
        transaction,
        conventions,
        onExecuting,
        onExecuted
        );
    
    public static async Task<IEnumerable<TReturn>> GetFromRoutineAsync<TFirst, TSecond, TThird, TFourth, TReturn>(
        this IDbConnection connection,
        string routineName,
        DbRoutineType? routineType,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TReturn> map,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionAsyncHandler<IEnumerable<TReturn>>? onExecuted = null
    )
    {
        var commandDefinition = BuildCommandDefinition(routineName, routineType, parameters as object, true, transaction, conventions);
        
        onExecuting?.Invoke(commandDefinition);
        var result = await connection.QueryAsync(
            commandDefinition.CommandText,
            map,
            commandDefinition.Parameters,
            transaction,
            commandDefinition.Flags == CommandFlags.Buffered,
            splitOn,
            commandDefinition.CommandTimeout,
            commandDefinition.CommandType
            );
        return onExecuted is not null ? await onExecuted(commandDefinition, result) : result;
    }
    
    public static Task<IEnumerable<TReturn>> GetFromRoutineAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(
        this IDbConnection connection,
        string routineName,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionAsyncHandler<IEnumerable<TReturn>>? onExecuted = null
    ) => connection.GetFromRoutineAsync(
        routineName,
        null,
        splitOn,
        map,
        parameters as object,
        transaction,
        conventions,
        onExecuting,
        onExecuted
        );
    
    public static async Task<IEnumerable<TReturn>> GetFromRoutineAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(
        this IDbConnection connection,
        string routineName,
        DbRoutineType? routineType,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionAsyncHandler<IEnumerable<TReturn>>? onExecuted = null
    )
    {
        var commandDefinition = BuildCommandDefinition(routineName, routineType, parameters as object, true, transaction, conventions);
        
        onExecuting?.Invoke(commandDefinition);
        var result = await connection.QueryAsync(
            commandDefinition.CommandText,
            map,
            commandDefinition.Parameters,
            transaction,
            commandDefinition.Flags == CommandFlags.Buffered,
            splitOn,
            commandDefinition.CommandTimeout,
            commandDefinition.CommandType
            );
        return onExecuted is not null ? await onExecuted(commandDefinition, result) : result;
    }
    
    public static Task<IEnumerable<TReturn>> GetFromRoutineAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(
        this IDbConnection connection,
        string routineName,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionAsyncHandler<IEnumerable<TReturn>>? onExecuted = null
    ) => connection.GetFromRoutineAsync(
        routineName,
        null,
        splitOn,
        map,
        parameters as object,
        transaction,
        conventions,
        onExecuting,
        onExecuted
        );
    
    public static async Task<IEnumerable<TReturn>> GetFromRoutineAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(
        this IDbConnection connection,
        string routineName,
        DbRoutineType? routineType,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionAsyncHandler<IEnumerable<TReturn>>? onExecuted = null
    )
    {
        var commandDefinition = BuildCommandDefinition(routineName, routineType, parameters as object, true, transaction, conventions);
        
        onExecuting?.Invoke(commandDefinition);
        var result = await connection.QueryAsync(
            commandDefinition.CommandText,
            map,
            commandDefinition.Parameters,
            transaction,
            commandDefinition.Flags == CommandFlags.Buffered,
            splitOn,
            commandDefinition.CommandTimeout,
            commandDefinition.CommandType
            );
        return onExecuted is not null ? await onExecuted(commandDefinition, result) : result;
    }
    
    public static Task<IEnumerable<TReturn>> GetFromRoutineAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(
        this IDbConnection connection,
        string routineName,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionAsyncHandler<IEnumerable<TReturn>>? onExecuted = null
    ) => connection.GetFromRoutineAsync(
        routineName,
        null,
        splitOn,
        map,
        parameters as object,
        transaction,
        conventions,
        onExecuting,
        onExecuted
        );
    
    /// <summary>
    /// Asynchronously executes a stored procedure or function and maps the result to multiple objects.
    /// </summary>
    /// <param name="connection">
    /// The database connection.
    /// </param>
    /// <param name="routineName">
    /// The name of the stored procedure or function.
    /// </param>
    /// <param name="routineType">
    /// A value of <see cref="DbRoutineType"/> indicating the type of routine (e.g., stored procedure or function).
    /// </param>
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the stored procedure or function.
    /// </param>
    /// <param name="transaction">
    /// The database transaction to use.
    /// </param>
    /// <param name="conventions">
    /// The conventions to use for invoking the routine.
    /// </param>
    /// <param name="onExecuting">
    /// A callback function that is executed before the database command is invoked.
    /// </param>
    /// <param name="onExecuted">
    /// A callback function that is executed after the database command is invoked.
    /// </param>
    /// <typeparam name="TFirst">
    /// The type of the first object in the result set.
    /// </typeparam>
    /// <typeparam name="TSecond">
    /// The type of the second object in the result set.
    /// </typeparam>
    /// <typeparam name="TThird">
    /// The type of the third object in the result set.
    /// </typeparam>
    /// <typeparam name="TFourth">
    /// The type of the fourth object in the result set.
    /// </typeparam>
    /// <typeparam name="TFifth">
    /// The type of the fifth object in the result set.
    /// </typeparam>
    /// <typeparam name="TSixth">
    /// The type of the sixth object in the result set.
    /// </typeparam>
    /// <typeparam name="TSeventh">
    /// The type of the seventh object in the result set.
    /// </typeparam>
    /// <typeparam name="TReturn">
    /// The type of the object to return.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of the mapped objects.
    /// </returns>
    public static async Task<IEnumerable<TReturn>> GetFromRoutineAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(
        this IDbConnection connection,
        string routineName,
        DbRoutineType? routineType,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionAsyncHandler<IEnumerable<TReturn>>? onExecuted = null
    )
    {
        var commandDefinition = BuildCommandDefinition(routineName, routineType, parameters as object, true, transaction, conventions);
        
        onExecuting?.Invoke(commandDefinition);
        var result = await connection.QueryAsync(
            commandDefinition.CommandText,
            map,
            commandDefinition.Parameters,
            transaction,
            commandDefinition.Flags == CommandFlags.Buffered,
            splitOn,
            commandDefinition.CommandTimeout,
            commandDefinition.CommandType
            );
        return onExecuted is not null ? await onExecuted(commandDefinition, result) : result;
    }
}