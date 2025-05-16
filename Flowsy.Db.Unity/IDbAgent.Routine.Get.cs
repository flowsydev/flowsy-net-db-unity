namespace Flowsy.Db.Unity;

public partial interface IDbAgent
{
    /// <summary>
    /// Executes a database routine (stored procedure or function) and returns the result as an enumerable collection of type T.
    /// </summary>
    /// <param name="routineName">
    /// The name of the routine to execute.
    /// The type of the routine (stored procedure or function) will be resolved from the connection options associated whith this agent.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the routine. This can be an anonymous object or a dictionary.
    /// </param>
    /// <typeparam name="T">
    /// The type to map the results to.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of type T containing the result set returned by the routine.
    /// </returns>
    IEnumerable<T> GetFromRoutine<T>(string routineName, dynamic? parameters = null);
    
    /// <summary>
    /// Executes a database routine (stored procedure or function) and returns the result as an enumerable collection of type T.
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
    /// The type to map the results to.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of type T containing the result set returned by the routine.
    /// </returns>
    IEnumerable<T> GetFromRoutine<T>(string routineName, DbRoutineType? routineType, dynamic? parameters = null);
    
    /// <summary>
    /// Asynchronously executes a database routine (stored procedure or function) and returns the result as an enumerable collection of type T.
    /// </summary>
    /// <param name="routineName">
    /// The name of the routine to execute.
    /// The type of the routine (stored procedure or function) will be resolved from the connection options associated whith this agent.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the routine. This can be an anonymous object or a dictionary.
    /// </param>
    /// <param name="cancellationToken">
    /// An optional cancellation token for the asynchronous operation.
    /// </param>
    /// <typeparam name="T">
    /// The type to map the results to.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of type T containing the result set returned by the routine.
    /// </returns>
    Task<IEnumerable<T>> GetFromRoutineAsync<T>(string routineName, dynamic? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Asynchronously executes a database routine (stored procedure or function) and returns the result as an enumerable collection of type T.
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
    /// The type to map the results to.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of type T containing the result set returned by the routine.
    /// </returns>
    Task<IEnumerable<T>> GetFromRoutineAsync<T>(string routineName, DbRoutineType? routineType, dynamic? parameters = null, CancellationToken cancellationToken = default);
}