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