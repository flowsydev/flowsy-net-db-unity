namespace Flowsy.Db.Unity;

public partial interface IDbAgent
{
    T GetSingleFromRoutine<T>(string routineName, dynamic? parameters = null);
    T GetSingleFromRoutine<T>(string routineName, DbRoutineType? routineType, dynamic? parameters = null);
    
    Task<T> GetSingleFromRoutineAsync<T>(string routineName, dynamic? parameters = null, CancellationToken cancellationToken = default);
    Task<T> GetSingleFromRoutineAsync<T>(string routineName, DbRoutineType? routineType, dynamic? parameters = null, CancellationToken cancellationToken = default);
}