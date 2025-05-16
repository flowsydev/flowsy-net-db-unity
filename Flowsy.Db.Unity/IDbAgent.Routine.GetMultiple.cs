using Dapper;

namespace Flowsy.Db.Unity;

public partial interface IDbAgent
{
    /// <summary>
    /// Executes a database routine (stored procedure or function) that returns multiple result sets.
    /// </summary>
    /// <param name="routineName">
    /// The name of the routine to execute.
    /// The type of the routine (stored procedure or function) will be resolved from the connection options associated with this agent.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the routine. This can be an anonymous object or a dictionary.
    /// </param>
    /// <returns>
    /// A <see cref="SqlMapper.GridReader"/> that contains the result sets returned by the routine.
    /// </returns>
    SqlMapper.GridReader GetMultipleFromRoutine(string routineName, dynamic? parameters = null);

    /// <summary>
    /// Executes a database routine (stored procedure or function) that returns multiple result sets.
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
    /// A <see cref="SqlMapper.GridReader"/> that contains the result sets returned by the routine.
    /// </returns>
    SqlMapper.GridReader GetMultipleFromRoutine(string routineName, DbRoutineType? routineType, dynamic? parameters = null);
    
    /// <summary>
    /// Asynchronously executes a database routine (stored procedure or function) that returns multiple result sets.
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
    /// A <see cref="SqlMapper.GridReader"/> that contains the result sets returned by the routine.
    /// </returns>
    Task<SqlMapper.GridReader> GetMultipleFromRoutineAsync(string routineName, dynamic? parameters = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously executes a database routine (stored procedure or function) that returns multiple result sets.
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
    /// A <see cref="SqlMapper.GridReader"/> that contains the result sets returned by the routine.
    /// </returns>
    Task<SqlMapper.GridReader> GetMultipleFromRoutineAsync(string routineName, DbRoutineType? routineType, dynamic? parameters = null, CancellationToken cancellationToken = default);
}