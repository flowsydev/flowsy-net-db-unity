using System.Data;
using Dapper;
using Flowsy.Db.Unity.Conventions;

namespace Flowsy.Db.Unity.Extensions;

public static partial class DbConnectionExtensions
{
    public static IEnumerable<TReturn> GetFromStatement<TFirst, TSecond, TReturn>(
        this IDbConnection connection,
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TReturn> map,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionHandler<IEnumerable<TReturn>>? onExecuted = null
        )
    {
        var finalConventions = conventions ?? DbConventionSet.Default;
        var commandDefinition = finalConventions.Commands.BuildDefinition(commandText, parameters as object, transaction);
        
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
    
    public static IEnumerable<TReturn> GetFromStatement<TFirst, TSecond, TThird, TReturn>(
        this IDbConnection connection,
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TReturn> map,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionHandler<IEnumerable<TReturn>>? onExecuted = null
        )
    {
        var finalConventions = conventions ?? DbConventionSet.Default;
        var commandDefinition = finalConventions.Commands.BuildDefinition(commandText, parameters as object, transaction);
        
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
    
    public static IEnumerable<TReturn> GetFromStatement<TFirst, TSecond, TThird, TFourth, TReturn>(
        this IDbConnection connection,
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TReturn> map,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionHandler<IEnumerable<TReturn>>? onExecuted = null
        )
    {
        var finalConventions = conventions ?? DbConventionSet.Default;
        var commandDefinition = finalConventions.Commands.BuildDefinition(commandText, parameters as object, transaction);
        
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
    
    public static IEnumerable<TReturn> GetFromStatement<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(
        this IDbConnection connection,
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionHandler<IEnumerable<TReturn>>? onExecuted = null
        )
    {
        var finalConventions = conventions ?? DbConventionSet.Default;
        var commandDefinition = finalConventions.Commands.BuildDefinition(commandText, parameters as object, transaction);
        
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
    
    public static IEnumerable<TReturn> GetFromStatement<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(
        this IDbConnection connection,
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionHandler<IEnumerable<TReturn>>? onExecuted = null
        )
    {
        var finalConventions = conventions ?? DbConventionSet.Default;
        var commandDefinition = finalConventions.Commands.BuildDefinition(commandText, parameters as object, transaction);
        
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
    
    public static IEnumerable<TReturn> GetFromStatement<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(
        this IDbConnection connection,
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionHandler<IEnumerable<TReturn>>? onExecuted = null
        )
    {
        var finalConventions = conventions ?? DbConventionSet.Default;
        var commandDefinition = finalConventions.Commands.BuildDefinition(commandText, parameters as object, transaction);
        
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
    
    public static async Task<IEnumerable<TReturn>> GetFromStatementAsync<TFirst, TSecond, TReturn>(
        this IDbConnection connection,
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TReturn> map,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionAsyncHandler<IEnumerable<TReturn>>? onExecuted = null
        )
    {
        var finalConventions = conventions ?? DbConventionSet.Default;
        var commandDefinition = finalConventions.Commands.BuildDefinition(commandText, parameters as object, transaction);
        
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
    
    public static async Task<IEnumerable<TReturn>> GetFromStatementAsync<TFirst, TSecond, TThird, TReturn>(
        this IDbConnection connection,
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TReturn> map,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionAsyncHandler<IEnumerable<TReturn>>? onExecuted = null
        )
    {
        var finalConventions = conventions ?? DbConventionSet.Default;
        var commandDefinition = finalConventions.Commands.BuildDefinition(commandText, parameters as object, transaction);
        
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
    
    public static async Task<IEnumerable<TReturn>> GetFromStatementAsync<TFirst, TSecond, TThird, TFourth, TReturn>(
        this IDbConnection connection,
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TReturn> map,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionAsyncHandler<IEnumerable<TReturn>>? onExecuted = null
        )
    {
        var finalConventions = conventions ?? DbConventionSet.Default;
        var commandDefinition = finalConventions.Commands.BuildDefinition(commandText, parameters as object, transaction);
        
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
    
    public static async Task<IEnumerable<TReturn>> GetFromStatementAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(
        this IDbConnection connection,
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionAsyncHandler<IEnumerable<TReturn>>? onExecuted = null
        )
    {
        var finalConventions = conventions ?? DbConventionSet.Default;
        var commandDefinition = finalConventions.Commands.BuildDefinition(commandText, parameters as object, transaction);
        
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
    
    public static async Task<IEnumerable<TReturn>> GetFromStatementAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(
        this IDbConnection connection,
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionAsyncHandler<IEnumerable<TReturn>>? onExecuted = null
        )
    {
        var finalConventions = conventions ?? DbConventionSet.Default;
        var commandDefinition = finalConventions.Commands.BuildDefinition(commandText, parameters as object, transaction);
        
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
    
    public static async Task<IEnumerable<TReturn>> GetFromStatementAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(
        this IDbConnection connection,
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionAsyncHandler<IEnumerable<TReturn>>? onExecuted = null
        )
    {
        var finalConventions = conventions ?? DbConventionSet.Default;
        var commandDefinition = finalConventions.Commands.BuildDefinition(commandText, parameters as object, transaction);
        
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