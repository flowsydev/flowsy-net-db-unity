using Dapper;
using Microsoft.Extensions.Logging;

namespace Flowsy.Db.Unity;

public partial class DbSession
{
    /// <summary>
    /// Performs a query to a stored procedure or function in the database and returns the first result.
    /// Throws an exception if no results are found.
    /// </summary>
    /// <param name="routineName">
    /// Name of the stored procedure or function to execute.
    /// </param>
    /// <param name="parameters">
    /// Optional parameters for the routine.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <typeparam name="T">
    /// Type of the expected result from the query.
    /// </typeparam>
    /// <returns>
    /// A task that represents the asynchronous operation of querying the database.
    /// The result is the first element returned by the query.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the query returns no results.
    /// </exception>
    public Task<T> QueryFirstFromRoutineAsync<T>(
        string routineName,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
    )
        => QueryFirstFromRoutineAsync<T>(routineName, Configuration.Conventions.Routines.RoutineType, parameters, cancellationToken);

    /// <summary>
    /// Performs a query to a stored procedure or function in the database and returns the first result.
    /// Throws an exception if no results are found.
    /// </summary>
    /// <param name="routineName">
    /// Name of the stored procedure or function to execute.
    /// </param>
    /// <param name="routineType">
    /// Type of routine (stored procedure or function).
    /// </param>
    /// <param name="parameters">
    /// Optional parameters for the routine.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <typeparam name="T">
    /// Type of the expected result from the query.
    /// </typeparam>
    /// <returns>
    /// A task that represents the asynchronous operation of querying the database.
    /// The result is the first element returned by the query.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the query returns no results.
    /// </exception>
    public async Task<T> QueryFirstFromRoutineAsync<T>(
        string routineName,
        DbRoutineType routineType,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
    )
    {
        await EnsureOpenConnectionAsync(cancellationToken);

        var (routineConvention, commandConvention) = Configuration.Conventions;

        var commandDefinition = BuildCommandDefinition(
            routineName,
            routineType,
            parameters as object,
            true,
            routineConvention,
            commandConvention,
            cancellationToken
        );

        var operationId = CreateOperationId();
        
        _logger?.Log(
            Configuration.LogLevel,
            "[ SESSION:{SessionId} > OP:{OperationId} ] Executing QueryFirst routine: {RoutineCall}", 
            SessionId,
            operationId,
            commandDefinition.CommandText
        );

        try
        {
            var result = await _connection.QueryFirstAsync<T>(commandDefinition);

            _logger?.Log(
                Configuration.LogLevel,
                "[ SESSION:{SessionId} > OP:{OperationId} ] QueryFirst routine executed successfully",
                SessionId,
                operationId
            );

            return result;
        }
        catch (Exception exception)
        {
            _logger?.LogError(
                exception,
                "[ SESSION:{SessionId} > OP:{OperationId} ] Error executing QueryFirst routine",
                SessionId,
                operationId
            );
            throw;
        }
    }

    /// <summary>
    /// Performs a query to a stored procedure or function in the database and returns the first result, or the default value if no results are found.
    /// </summary>
    /// <param name="routineName">
    /// Name of the stored procedure or function to execute.
    /// </param>
    /// <param name="parameters">
    /// Optional parameters for the routine.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <typeparam name="T">
    /// Type of the expected result from the query.
    /// </typeparam>
    /// <returns>
    /// A task that represents the asynchronous operation of querying the database.
    /// The result is the first element returned by the query, or the default value of type T if no results are found.
    /// </returns>
    public Task<T?> QueryFirstOrDefaultFromRoutineAsync<T>(
        string routineName,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
    )
        => QueryFirstOrDefaultFromRoutineAsync<T>(routineName, Configuration.Conventions.Routines.RoutineType, parameters, cancellationToken);

    /// <summary>
    /// Performs a query to a stored procedure or function in the database and returns the first result, or the default value if no results are found.
    /// </summary>
    /// <param name="routineName">
    /// Name of the stored procedure or function to execute.
    /// </param>
    /// <param name="routineType">
    /// Type of routine (stored procedure or function).
    /// </param>
    /// <param name="parameters">
    /// Optional parameters for the routine.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <typeparam name="T">
    /// Type of the expected result from the query.
    /// </typeparam>
    /// <returns>
    /// A task that represents the asynchronous operation of querying the database.
    /// The result is the first element returned by the query, or the default value of type T if no results are found.
    /// </returns>
    public async Task<T?> QueryFirstOrDefaultFromRoutineAsync<T>(
        string routineName,
        DbRoutineType routineType,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
    )
    {
        await EnsureOpenConnectionAsync(cancellationToken);

        var (routineConvention, commandConvention) = Configuration.Conventions;

        var commandDefinition = BuildCommandDefinition(
            routineName,
            routineType,
            parameters as object,
            true,
            routineConvention,
            commandConvention,
            cancellationToken
        );

        var operationId = CreateOperationId();
        
        _logger?.Log(
            Configuration.LogLevel,
            "[ SESSION:{SessionId} > OP:{OperationId} ] Executing QueryFirstOrDefault routine: {RoutineCall}", 
            SessionId,
            operationId,
            commandDefinition.CommandText
        );

        try
        {
            var result = await _connection.QueryFirstOrDefaultAsync<T>(commandDefinition);

            _logger?.Log(
                Configuration.LogLevel,
                "[ SESSION:{SessionId} > OP:{OperationId} ] QueryFirstOrDefault routine executed successfully",
                SessionId,
                operationId
            );

            return result;
        }
        catch (Exception exception)
        {
            _logger?.LogError(
                exception,
                "[ SESSION:{SessionId} > OP:{OperationId} ] Error executing QueryFirstOrDefault routine",
                SessionId,
                operationId
            );
            throw;
        }
    }
}
