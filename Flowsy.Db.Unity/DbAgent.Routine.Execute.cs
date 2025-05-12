using Flowsy.Db.Unity.Extensions;

namespace Flowsy.Db.Unity;

public partial class DbAgent
{
    public int ExecuteRoutine(string routineName, dynamic? parameters = null)
        => ExecuteRoutine(routineName, null, parameters as object);
    
    public int ExecuteRoutine(string routineName, DbRoutineType? routineType, dynamic? parameters = null)
        => Connection.ExecuteRoutine(
            routineName,
            routineType,
            parameters as object,
            UnitOfWork?.Transaction,
            ConnectionOptions.Conventions,
            c => OnCommandExecuting(new DbCommandExecutingEventArgs(c, Connection, UnitOfWork?.Transaction)),
            (c, r) =>
            {
                OnCommandExecuted(new DbCommandExecutedEventArgs(c, Connection, UnitOfWork?.Transaction, r));
                return r;
            }
        );
    
    public Task<int> ExecuteRoutineAsync(string routineName, dynamic? parameters = null, CancellationToken cancellationToken = default)
        => ExecuteRoutineAsync(routineName, null, parameters as object, cancellationToken);

    public Task<int> ExecuteRoutineAsync(string routineName, DbRoutineType? routineType, dynamic? parameters = null, CancellationToken cancellationToken = default)
        => Connection.ExecuteRoutineAsync(
            routineName,
            routineType,
            parameters as object,
            UnitOfWork?.Transaction,
            ConnectionOptions.Conventions,
            c => OnCommandExecuting(new DbCommandExecutingEventArgs(c, Connection, UnitOfWork?.Transaction)),
            (c, r) =>
            {
                OnCommandExecuted(new DbCommandExecutedEventArgs(c, Connection, UnitOfWork?.Transaction, r));
                return Task.FromResult(r);
            },
            cancellationToken
        );
}