namespace Flowsy.Db.Unity;

public partial interface IDbAgent
{
    int ExecuteRoutine(string routineName, dynamic? parameters = null);
    int ExecuteRoutine(string routineName, DbRoutineType? routineType = null, dynamic? parameters = null);

    Task<int> ExecuteRoutineAsync(string routineName, dynamic? parameters = null, CancellationToken cancellationToken = default);
    Task<int> ExecuteRoutineAsync(string routineName, DbRoutineType? routineType, dynamic? parameters = null, CancellationToken cancellationToken = default);
}