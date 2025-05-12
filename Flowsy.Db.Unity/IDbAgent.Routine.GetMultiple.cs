using Dapper;

namespace Flowsy.Db.Unity;

public partial interface IDbAgent
{
    SqlMapper.GridReader GetMultipleFromRoutine(string routineName, dynamic? parameters = null);
    SqlMapper.GridReader GetMultipleFromRoutine(string routineName, DbRoutineType? routineType, dynamic? parameters = null);
    
    Task<SqlMapper.GridReader> GetMultipleFromRoutineAsync(string routineName, dynamic? parameters = null, CancellationToken cancellationToken = default);
    Task<SqlMapper.GridReader> GetMultipleFromRoutineAsync(string routineName, DbRoutineType? routineType, dynamic? parameters = null, CancellationToken cancellationToken = default);
}