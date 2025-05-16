namespace Flowsy.Db.Unity;

public partial interface IDbAgent
{
    /// <summary>
    /// Executes a database routine (stored procedure or function) with the specified name and parameters.
    /// </summary>
    /// <param name="routineName">
    /// The name of the routine to execute.
    /// The type of the routine (stored procedure or function) will be resolved from the connection options associated with this agent.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the routine. This can be an anonymous object or a dictionary.
    /// </param>
    /// <returns>
    /// The number of rows affected by the routine execution.
    /// </returns>
    int ExecuteRoutine(string routineName, dynamic? parameters = null);
    
    /// <summary>
    /// Executes a database routine (stored procedure or function) with the specified name and parameters.
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
    /// <returns>
    /// The number of rows affected by the routine execution.
    /// </returns>
    int ExecuteRoutine(string routineName, DbRoutineType? routineType = null, dynamic? parameters = null);

    /// <summary>
    /// Asynchronously executes a database routine (stored procedure or function) with the specified name and parameters.
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
    /// <returns>
    /// The number of rows affected by the routine execution.
    /// </returns>
    Task<int> ExecuteRoutineAsync(string routineName, dynamic? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Asynchronously executes a database routine (stored procedure or function) with the specified name and parameters.
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
    /// <returns>
    /// The number of rows affected by the routine execution.
    /// </returns>
    Task<int> ExecuteRoutineAsync(string routineName, DbRoutineType? routineType, dynamic? parameters = null, CancellationToken cancellationToken = default);
}