namespace Flowsy.Db.Unity;

public partial interface IDbAgent
{
    IEnumerable<T> GetFromRoutine<T>(string routineName, dynamic? parameters = null);
    IEnumerable<T> GetFromRoutine<T>(string routineName, DbRoutineType? routineType, dynamic? parameters = null);
    
    Task<IEnumerable<T>> GetFromRoutineAsync<T>(string routineName, dynamic? parameters = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetFromRoutineAsync<T>(string routineName, DbRoutineType? routineType, dynamic? parameters = null, CancellationToken cancellationToken = default);
}