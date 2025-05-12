namespace Flowsy.Db.Unity;

public partial interface IDbAgent
{
    T? GetFirstOrDefaultFromRoutine<T>(string routineName, dynamic? parameters = null);
    T? GetFirstOrDefaultFromRoutine<T>(string routineName, DbRoutineType? routineType, dynamic? parameters = null);
    
    Task<T?> GetFirstOrDefaultFromRoutineAsync<T>(string routineName, dynamic? parameters = null, CancellationToken cancellationToken = default);
    Task<T?> GetFirstOrDefaultFromRoutineAsync<T>(string routineName, DbRoutineType? routineType, dynamic? parameters = null, CancellationToken cancellationToken = default);
}