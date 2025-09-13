using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;

namespace Flowsy.Db.Unity;

public partial class DbSession
{
    /// <summary>
    /// Performs a database query and returns an enumerable of results.
    /// </summary>
    /// <param name="statement">
    /// SQL statement to execute.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <typeparam name="T">
    /// Type of the expected results from the query.
    /// </typeparam>
    /// <returns>
    /// A task that represents the asynchronous database query operation.
    /// </returns>
    public Task<IEnumerable<T>> QueryAsync<T>(
        string statement,
        CancellationToken cancellationToken = default
        ) => QueryAsync<T>(statement, null, cancellationToken);
    
    /// <summary>
    /// Performs a database query and returns an enumerable of results.
    /// </summary>
    /// <param name="statement">
    /// SQL statement to execute.
    /// </param>
    /// <param name="parameters">
    /// Optional parameters for the query, which can be used to prevent SQL injection.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <typeparam name="T">
    /// Type of the expected results from the query.
    /// </typeparam>
    /// <returns>
    /// A task that represents the asynchronous database query operation.
    /// </returns>
    public async Task<IEnumerable<T>> QueryAsync<T>(
        string statement,
        dynamic? parameters,
        CancellationToken cancellationToken = default
        )
    {
        await EnsureOpenConnectionAsync(cancellationToken);

        var commandDefinition = BuildCommandDefinition(
            statement,
            parameters as object,
            CommandType.Text,
            Configuration.Conventions.Commands,
            cancellationToken
        );

        var operationId = CreateOperationId();
        
        _logger?.Log(
            Configuration.LogLevel,
            "[ SESSION:{SessionId} > OP:{OperationId} ] Executing statement: {StatementText}", 
            SessionId,
            operationId,
            commandDefinition.CommandText
        );

        try
        {
            var results = await _connection.QueryAsync<T>(commandDefinition);

            _logger?.Log(
                Configuration.LogLevel,
                "[ SESSION:{SessionId} > OP:{OperationId} ] Statement executed",
                SessionId,
                operationId
            );

            return results;
        }
        catch (Exception exception)
        {
            _logger?.LogError(
                exception,
                "[ SESSION:{SessionId} > OP:{OperationId} ] Error executing statement",
                SessionId,
                operationId
            );
            throw;
        }
    }

    /// <summary>
    /// Performs a query to a stored procedure or function in the database and returns an enumerable of results.
    /// </summary>
    /// <param name="routineName">
    /// Name of the stored procedure or function to execute.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <typeparam name="T">
    /// Type of the expected results from the query.
    /// </typeparam>
    /// <returns>
    /// A task that represents the asynchronous database query operation.
    /// </returns>
    public Task<IEnumerable<T>> QueryFromRoutineAsync<T>(string routineName, CancellationToken cancellationToken = default)
        => QueryFromRoutineAsync<T>(routineName, Configuration.Conventions.Routines.RoutineType, null, cancellationToken);

    /// <summary>
    /// Performs a query to a stored procedure or function in the database and returns an enumerable of results.
    /// </summary>
    /// <param name="routineName">
    /// Name of the stored procedure or function to execute.
    /// </param>
    /// <param name="parameters">
    /// Optional parameters for the query.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <typeparam name="T">
    /// Type of the expected results from the query.
    /// </typeparam>
    /// <returns>
    /// A task that represents the asynchronous database query operation.
    /// </returns>
    public Task<IEnumerable<T>> QueryFromRoutineAsync<T>(
        string routineName,
        dynamic? parameters,
        CancellationToken cancellationToken = default
        )
        => QueryFromRoutineAsync<T>(routineName, Configuration.Conventions.Routines.RoutineType, parameters, cancellationToken);

    /// <summary>
    /// Performs a query to a stored procedure or function in the database and returns an enumerable of results.
    /// </summary>
    /// <param name="routineName">
    /// Name of the stored procedure or function to execute.
    /// </param>
    /// <param name="routineType">
    /// Type of routine (stored procedure or function).
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <typeparam name="T">
    /// Type of the expected results from the query.
    /// </typeparam>
    /// <returns>
    /// A task that represents the asynchronous database query operation.
    /// </returns>
    public Task<IEnumerable<T>> QueryFromRoutineAsync<T>(
        string routineName,
        DbRoutineType routineType,
        CancellationToken cancellationToken = default
        )
        => QueryFromRoutineAsync<T>(routineName, routineType, null, cancellationToken);

    /// <summary>
    /// Performs a query to a stored procedure or function in the database and returns an enumerable of results.
    /// </summary>
    /// <param name="routineName">
    /// Name of the stored procedure or function to execute.
    /// </param>
    /// <param name="routineType">
    /// Type of routine (stored procedure or function).
    /// </param>
    /// <param name="parameters">
    /// Optional parameters for the query.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <typeparam name="T">
    /// Type of the expected results from the query.
    /// </typeparam>
    /// <returns>
    /// A task that represents the asynchronous database query operation.
    /// </returns>
    public async Task<IEnumerable<T>> QueryFromRoutineAsync<T>(
        string routineName,
        DbRoutineType routineType,
        dynamic? parameters,
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
            "[ SESSION:{SessionId} > OP:{OperationId} ] Executing routine: {RoutineCall}",
            SessionId,
            operationId,
            commandDefinition.CommandText
            );

        try
        {
            var results = await _connection.QueryAsync<T>(commandDefinition);

            _logger?.Log(
                Configuration.LogLevel,
                "[ SESSION:{SessionId} > OP:{OperationId} ] Routine executed",
                SessionId,
                operationId
                );

            return results;
        }
        catch (Exception exception)
        {
            _logger?.LogError(
                exception,
                "[ SESSION:{SessionId} > OP:{OperationId} ] Error executing routine",
                SessionId,
                operationId
                );
            
            throw;
        }
    }
}