using System.Data;
using Dapper;
using Flowsy.Db.Unity.Conventions;

namespace Flowsy.Db.Unity.Extensions;

public static partial class DbConnectionExtensions
{
    /// <summary>
    /// Executes a database routine (stored procedure or function) and maps the result to multiple objects.
    /// </summary>
    /// <param name="connection">
    /// The database connection to use for executing the routine.
    /// </param>
    /// <param name="routineName">
    /// The name of the routine to execute.
    /// The type of the routine (stored procedure or function) will be resolved from the provided conventions.
    /// </param>
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
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
    /// <typeparam name="TFirst">
    /// The type of the first object in the result set.
    /// </typeparam>
    /// <typeparam name="TSecond">
    /// The type of the second object in the result set.
    /// </typeparam>
    /// <typeparam name="TReturn">
    /// The type of the object to return.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of the mapped objects.
    /// </returns>
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

    /// <summary>
    /// Executes a database routine (stored procedure or function) and maps the result to multiple objects.
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
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
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
    /// <typeparam name="TFirst">
    /// The type of the first object in the result set.
    /// </typeparam>
    /// <typeparam name="TSecond">
    /// The type of the second object in the result set.
    /// </typeparam>
    /// <typeparam name="TReturn">
    /// The type of the object to return.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of the mapped objects.
    /// </returns>
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

    /// <summary>
    /// Executes a database routine (stored procedure or function) and maps the result to multiple objects.
    /// </summary>
    /// <param name="connection">
    /// The database connection to use for executing the routine.
    /// </param>
    /// <param name="routineName">
    /// The name of the routine to execute.
    /// The type of the routine (stored procedure or function) will be resolved from the provided conventions.
    /// </param>
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
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
    /// <typeparam name="TFirst">
    /// The type of the first object in the result set.
    /// </typeparam>
    /// <typeparam name="TSecond">
    /// The type of the second object in the result set.
    /// </typeparam>
    /// <typeparam name="TThird">
    /// The type of the third object in the result set.
    /// </typeparam>
    /// <typeparam name="TReturn">
    /// The type of the object to return.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of the mapped objects.
    /// </returns>
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

    /// <summary>
    /// Executes a database routine (stored procedure or function) and maps the result to multiple objects.
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
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
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
    /// <typeparam name="TFirst">
    /// The type of the first object in the result set.
    /// </typeparam>
    /// <typeparam name="TSecond">
    /// The type of the second object in the result set.
    /// </typeparam>
    /// <typeparam name="TThird">
    /// The type of the third object in the result set.
    /// </typeparam>
    /// <typeparam name="TReturn">
    /// The type of the object to return.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of the mapped objects.
    /// </returns>
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

    /// <summary>
    /// Executes a database routine (stored procedure or function) and maps the result to multiple objects.
    /// </summary>
    /// <param name="connection">
    /// The database connection to use for executing the routine.
    /// </param>
    /// <param name="routineName">
    /// The name of the routine to execute.
    /// The type of the routine (stored procedure or function) will be resolved from the provided conventions.
    /// </param>
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
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
    /// <typeparam name="TReturn">
    /// The type of the object to return.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of the mapped objects.
    /// </returns>
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

    /// <summary>
    /// Executes a database routine (stored procedure or function) and maps the result to multiple objects.
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
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
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
    /// <typeparam name="TReturn">
    /// The type of the object to return.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of the mapped objects.
    /// </returns>
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

    /// <summary>
    /// Executes a database routine (stored procedure or function) and maps the result to multiple objects.
    /// </summary>
    /// <param name="connection">
    /// The database connection to use for executing the routine.
    /// </param>
    /// <param name="routineName">
    /// The name of the routine to execute.
    /// The type of the routine (stored procedure or function) will be resolved from the provided conventions.
    /// </param>
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
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
    /// <typeparam name="TReturn">
    /// The type of the object to return.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of the mapped objects.
    /// </returns>
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

    /// <summary>
    /// Executes a database routine (stored procedure or function) and maps the result to multiple objects.
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
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
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
    /// <typeparam name="TReturn">
    /// The type of the object to return.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of the mapped objects.
    /// </returns>
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

    /// <summary>
    /// Executes a database routine (stored procedure or function) and maps the result to multiple objects.
    /// </summary>
    /// <param name="connection">
    /// The database connection to use for executing the routine.
    /// </param>
    /// <param name="routineName">
    /// The name of the routine to execute.
    /// The type of the routine (stored procedure or function) will be resolved from the provided conventions.
    /// </param>
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
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
    /// <typeparam name="TReturn">
    /// The type of the object to return.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of the mapped objects.
    /// </returns>
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

    /// <summary>
    /// Executes a database routine (stored procedure or function) and maps the result to multiple objects.
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
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
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
    /// <typeparam name="TReturn">
    /// The type of the object to return.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of the mapped objects.
    /// </returns>
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

    /// <summary>
    /// Executes a database routine (stored procedure or function) and maps the result to multiple objects.
    /// </summary>
    /// <param name="connection">
    /// The database connection to use for executing the routine.
    /// </param>
    /// <param name="routineName">
    /// The name of the routine to execute.
    /// The type of the routine (stored procedure or function) will be resolved from the provided conventions.
    /// </param>
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
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
    /// Executes a database routine (stored procedure or function) and maps the result to multiple objects.
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
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
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

    /// <summary>
    /// Asynchronously executes a database routine (stored procedure or function) and maps the result to multiple objects.
    /// </summary>
    /// <param name="connection">
    /// The database connection to use for executing the routine.
    /// </param>
    /// <param name="routineName">
    /// The name of the routine to execute.
    /// The type of the routine (stored procedure or function) will be resolved from the provided conventions.
    /// </param>
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
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
    /// <typeparam name="TFirst">
    /// The type of the first object in the result set.
    /// </typeparam>
    /// <typeparam name="TSecond">
    /// The type of the second object in the result set.
    /// </typeparam>
    /// <typeparam name="TReturn">
    /// The type of the object to return.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of the mapped objects.
    /// </returns>
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

    /// <summary>
    /// Asynchronously executes a database routine (stored procedure or function) and maps the result to multiple objects.
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
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
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
    /// <typeparam name="TFirst">
    /// The type of the first object in the result set.
    /// </typeparam>
    /// <typeparam name="TSecond">
    /// The type of the second object in the result set.
    /// </typeparam>
    /// <typeparam name="TReturn">
    /// The type of the object to return.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of the mapped objects.
    /// </returns>
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

    /// <summary>
    /// Asynchronously executes a database routine (stored procedure or function) and maps the result to multiple objects.
    /// </summary>
    /// <param name="connection">
    /// The database connection to use for executing the routine.
    /// </param>
    /// <param name="routineName">
    /// The name of the routine to execute.
    /// The type of the routine (stored procedure or function) will be resolved from the provided conventions.
    /// </param>
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
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
    /// <typeparam name="TFirst">
    /// The type of the first object in the result set.
    /// </typeparam>
    /// <typeparam name="TSecond">
    /// The type of the second object in the result set.
    /// </typeparam>
    /// <typeparam name="TThird">
    /// The type of the third object in the result set.
    /// </typeparam>
    /// <typeparam name="TReturn">
    /// The type of the object to return.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of the mapped objects.
    /// </returns>
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

    /// <summary>
    /// Asynchronously executes a database routine (stored procedure or function) and maps the result to multiple objects.
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
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
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
    /// <typeparam name="TFirst">
    /// The type of the first object in the result set.
    /// </typeparam>
    /// <typeparam name="TSecond">
    /// The type of the second object in the result set.
    /// </typeparam>
    /// <typeparam name="TThird">
    /// The type of the third object in the result set.
    /// </typeparam>
    /// <typeparam name="TReturn">
    /// The type of the object to return.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of the mapped objects.
    /// </returns>
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

    /// <summary>
    /// Asynchronously executes a database routine (stored procedure or function) and maps the result to multiple objects.
    /// </summary>
    /// <param name="connection">
    /// The database connection to use for executing the routine.
    /// </param>
    /// <param name="routineName">
    /// The name of the routine to execute.
    /// The type of the routine (stored procedure or function) will be resolved from the provided conventions.
    /// </param>
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
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
    /// <typeparam name="TReturn">
    /// The type of the object to return.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of the mapped objects.
    /// </returns>
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

    /// <summary>
    /// Asynchronously executes a database routine (stored procedure or function) and maps the result to multiple objects.
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
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
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
    /// <typeparam name="TReturn">
    /// The type of the object to return.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of the mapped objects.
    /// </returns>
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

    /// <summary>
    /// Asynchronously executes a database routine (stored procedure or function) and maps the result to multiple objects.
    /// </summary>
    /// <param name="connection">
    /// The database connection to use for executing the routine.
    /// </param>
    /// <param name="routineName">
    /// The name of the routine to execute.
    /// The type of the routine (stored procedure or function) will be resolved from the provided conventions.
    /// </param>
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
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
    /// <typeparam name="TReturn">
    /// The type of the object to return.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of the mapped objects.
    /// </returns>
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

    /// <summary>
    /// Asynchronously executes a database routine (stored procedure or function) and maps the result to multiple objects.
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
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
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
    /// <typeparam name="TReturn">
    /// The type of the object to return.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of the mapped objects.
    /// </returns>
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

    /// <summary>
    /// Asynchronously executes a database routine (stored procedure or function) and maps the result to multiple objects.
    /// </summary>
    /// <param name="connection">
    /// The database connection to use for executing the routine.
    /// </param>
    /// <param name="routineName">
    /// The name of the routine to execute.
    /// The type of the routine (stored procedure or function) will be resolved from the provided conventions.
    /// </param>
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
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
    /// <typeparam name="TReturn">
    /// The type of the object to return.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of the mapped objects.
    /// </returns>
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

    /// <summary>
    /// Asynchronously executes a database routine (stored procedure or function) and maps the result to multiple objects.
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
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
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
    /// <typeparam name="TReturn">
    /// The type of the object to return.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of the mapped objects.
    /// </returns>
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

    /// <summary>
    /// Asynchronously executes a database routine (stored procedure or function) and maps the result to multiple objects.
    /// </summary>
    /// <param name="connection">
    /// The database connection to use for executing the routine.
    /// </param>
    /// <param name="routineName">
    /// The name of the routine to execute.
    /// The type of the routine (stored procedure or function) will be resolved from the provided conventions.
    /// </param>
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
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
    /// Asynchronously executes a database routine (stored procedure or function) and maps the result to multiple objects.
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
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
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
