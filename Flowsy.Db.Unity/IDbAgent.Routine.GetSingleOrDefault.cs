namespace Flowsy.Db.Unity;

public partial interface IDbAgent
{
    T? GetSingleOrDefaultFromRoutine<T>(string routineName, dynamic? parameters = null);
    T? GetSingleOrDefaultFromRoutine<T>(string routineName, DbRoutineType? routineType, dynamic? parameters = null);
    
    Task<T?> GetSingleOrDefaultFromRoutineAsync<T>(string routineName, dynamic? parameters = null, CancellationToken cancellationToken = default);
    Task<T?> GetSingleOrDefaultFromRoutineAsync<T>(string routineName, DbRoutineType? routineType, dynamic? parameters = null, CancellationToken cancellationToken = default);
}