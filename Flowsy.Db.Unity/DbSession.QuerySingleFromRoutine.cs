using Dapper;
using Microsoft.Extensions.Logging;

namespace Flowsy.Db.Unity;

public partial class DbSession
{
    /// <summary>
    /// Performs a query to a stored procedure or function in the database and returns exactly one result.
    /// Throws an exception if no results are found or if multiple results are found.
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
    /// A task that represents the asynchronous database query operation.
    /// The result is exactly one element returned by the query.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the query returns no results or returns multiple results.
    /// </exception>
    public Task<T> QuerySingleFromRoutineAsync<T>(
        string routineName,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
    )
        => QuerySingleFromRoutineAsync<T>(routineName, Configuration.Conventions.Routines.RoutineType, parameters, cancellationToken);

    /// <summary>
    /// Performs a query to a stored procedure or function in the database and returns exactly one result.
    /// Throws an exception if no results are found or if multiple results are found.
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
    /// A task that represents the asynchronous database query operation.
    /// The result is exactly one element returned by the query.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the query returns no results or returns multiple results.
    /// </exception>
    public async Task<T> QuerySingleFromRoutineAsync<T>(
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
            "[ SESSION:{SessionId} > OP:{OperationId} ] Executing QuerySingle routine: {RoutineCall}", 
            SessionId,
            operationId,
            commandDefinition.CommandText
        );

        try
        {
            var result = await _connection.QuerySingleAsync<T>(commandDefinition);

            _logger?.Log(
                Configuration.LogLevel,
                "[ SESSION:{SessionId} > OP:{OperationId} ] QuerySingle routine executed successfully",
                SessionId,
                operationId
            );

            return result;
        }
        catch (Exception exception)
        {
            _logger?.LogError(
                exception,
                "[ SESSION:{SessionId} > OP:{OperationId} ] Error executing QuerySingle routine",
                SessionId,
                operationId
            );
            throw;
        }
    }

    /// <summary>
    /// Performs a query to a stored procedure or function in the database and returns exactly one result, or the default value if no results are found.
    /// Throws an exception if multiple results are found.
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
    /// A task that represents the asynchronous database query operation.
    /// The result is exactly one element returned by the query, or the default value of type T if no results are found.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the query returns multiple results.
    /// </exception>
    public Task<T?> QuerySingleOrDefaultFromRoutineAsync<T>(
        string routineName,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
    )
        => QuerySingleOrDefaultFromRoutineAsync<T>(routineName, Configuration.Conventions.Routines.RoutineType, parameters, cancellationToken);

    /// <summary>
    /// Performs a query to a stored procedure or function in the database and returns exactly one result, or the default value if no results are found.
    /// Throws an exception if multiple results are found.
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
    /// A task that represents the asynchronous database query operation.
    /// The result is exactly one element returned by the query, or the default value of type T if no results are found.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the query returns multiple results.
    /// </exception>
    public async Task<T?> QuerySingleOrDefaultFromRoutineAsync<T>(
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
            "[ SESSION:{SessionId} > OP:{OperationId} ] Executing QuerySingleOrDefault routine: {RoutineCall}", 
            SessionId,
            operationId,
            commandDefinition.CommandText
        );

        try
        {
            var result = await _connection.QuerySingleOrDefaultAsync<T>(commandDefinition);

            _logger?.Log(
                Configuration.LogLevel,
                "[ SESSION:{SessionId} > OP:{OperationId} ] QuerySingleOrDefault routine executed successfully",
                SessionId,
                operationId
            );

            return result;
        }
        catch (Exception exception)
        {
            _logger?.LogError(
                exception,
                "[ SESSION:{SessionId} > OP:{OperationId} ] Error executing QuerySingleOrDefault routine",
                SessionId,
                operationId
            );
            throw;
        }
    }
}
