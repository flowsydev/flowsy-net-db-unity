using Flowsy.Db.Unity.Extensions;

namespace Flowsy.Db.Unity;

public partial class DbAgent
{
    /// <summary>
    /// Executes a database routine (stored procedure or function) and returns a single result as an instance of T.
    /// If a primitive type is required (int, string, etc.), then the first column of the single result found will be returned.
    /// If no results are found, the default value of T will be returned.
    /// If multiple results are found, an exception will be thrown. 
    /// </summary>
    /// <param name="routineName">
    /// The name of the routine to execute.
    /// The type of the routine (stored procedure or function) will be resolved from the connection options associated with this agent.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the routine. This can be an anonymous object or a dictionary.
    /// </param>
    /// <typeparam name="T">
    /// The type of the result to return. This can be a primitive type or a complex type.
    /// </typeparam>
    /// <returns>
    /// The single result of the routine execution as an instance of T.
    /// </returns>
    public T? GetSingleOrDefaultFromRoutine<T>(string routineName, dynamic? parameters = null)
        => GetSingleOrDefaultFromRoutine<T>(routineName, null, parameters as object);

    /// <summary>
    /// Executes a database routine (stored procedure or function) and returns a single result as an instance of T.
    /// If a primitive type is required (int, string, etc.), then the first column of the single result found will be returned.
    /// If no results are found, the default value of T will be returned.
    /// If multiple results are found, an exception will be thrown. 
    /// </summary>
    /// <param name="routineName">
    /// The name of the routine to execute.
    /// </param>
    /// <param name="routineType">
    /// The type of the routine (stored procedure or function). If null, the type will be resolved from the connection options associated with this agent.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the routine. This can be an anonymous object or a dictionary.
    /// </param>
    /// <typeparam name="T">
    /// The type of the result to return. This can be a primitive type or a complex type.
    /// </typeparam>
    /// <returns>
    /// The single result of the routine execution as an instance of T.
    /// </returns>
    public T? GetSingleOrDefaultFromRoutine<T>(string routineName, DbRoutineType? routineType, dynamic? parameters = null)
        => Connection.GetSingleOrDefaultFromRoutine<T>(
            routineName,
            routineType,
            parameters as object,
            UnitOfWork?.Transaction,
            ConnectionOptions.Conventions,
            c => OnCommandExecuting(new DbCommandExecutingEventArgs(c, Connection, UnitOfWork?.Transaction)),
            (c, r) =>
            {
                OnCommandExecuted(new DbCommandExecutedEventArgs(c, Connection, UnitOfWork?.Transaction, r));
                return r;
            }
        );

    /// <summary>
    /// Asynchronously executes a database routine (stored procedure or function) and returns a single result as an instance of T.
    /// If a primitive type is required (int, string, etc.), then the first column of the single result found will be returned.
    /// If no results are found, the default value of T will be returned.
    /// If multiple results are found, an exception will be thrown. 
    /// </summary>
    /// <param name="routineName">
    /// The name of the routine to execute.
    /// The type of the routine (stored procedure or function) will be resolved from the connection options associated with this agent.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the routine. This can be an anonymous object or a dictionary.
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
    public Task<T?> GetSingleOrDefaultFromRoutineAsync<T>(string routineName, dynamic? parameters = null, CancellationToken cancellationToken = default)
        => GetSingleOrDefaultFromRoutineAsync<T>(routineName, null, parameters as object, cancellationToken);

    /// <summary>
    /// Executes a database routine (stored procedure or function) and returns a single result as an instance of T.
    /// If a primitive type is required (int, string, etc.), then the first column of the single result found will be returned.
    /// If no results are found, the default value of T will be returned.
    /// If multiple results are found, an exception will be thrown. 
    /// </summary>
    /// <param name="routineName">
    /// The name of the routine to execute.
    /// </param>
    /// <param name="routineType">
    /// The type of the routine (stored procedure or function). If null, the type will be resolved from the connection options associated with this agent.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the routine. This can be an anonymous object or a dictionary.
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
    public Task<T?> GetSingleOrDefaultFromRoutineAsync<T>(string routineName, DbRoutineType? routineType, dynamic? parameters = null, CancellationToken cancellationToken = default)
        => Connection.GetSingleOrDefaultFromRoutineAsync<T>(
            routineName,
            routineType,
            parameters as object,
            UnitOfWork?.Transaction,
            ConnectionOptions.Conventions,
            c => OnCommandExecuting(new DbCommandExecutingEventArgs(c, Connection, UnitOfWork?.Transaction)),
            (c, r) =>
            {
                OnCommandExecuted(new DbCommandExecutedEventArgs(c, Connection, UnitOfWork?.Transaction, r));
                return Task.FromResult(r);
            },
            cancellationToken
        );
}