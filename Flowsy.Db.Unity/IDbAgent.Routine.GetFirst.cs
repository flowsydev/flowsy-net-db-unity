namespace Flowsy.Db.Unity;

public partial interface IDbAgent
{
    T GetFirstFromRoutine<T>(string routineName, dynamic? parameters = null);
    T GetFirstFromRoutine<T>(string routineName, DbRoutineType? routineType, dynamic? parameters = null);
    
    Task<T> GetFirstFromRoutineAsync<T>(string routineName, dynamic? parameters = null, CancellationToken cancellationToken = default);
    Task<T> GetFirstFromRoutineAsync<T>(string routineName, DbRoutineType? routineType, dynamic? parameters = null, CancellationToken cancellationToken = default);
}