using Flowsy.Db.Unity.Extensions;

namespace Flowsy.Db.Unity;

public partial class DbAgent
{
    public T GetFirstFromRoutine<T>(string routineName, dynamic? parameters = null)
        => GetFirstFromRoutine<T>(routineName, null, parameters as object);

    public T GetFirstFromRoutine<T>(string routineName, DbRoutineType? routineType, dynamic? parameters = null)
        => Connection.GetFirstFromRoutine<T>(
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

    public Task<T> GetFirstFromRoutineAsync<T>(string routineName, dynamic? parameters = null, CancellationToken cancellationToken = default)
        => GetFirstFromRoutineAsync<T>(routineName, null, parameters as object, cancellationToken);

    public Task<T> GetFirstFromRoutineAsync<T>(string routineName, DbRoutineType? routineType, dynamic? parameters = null, CancellationToken cancellationToken = default)
        => Connection.GetFirstFromRoutineAsync<T>(
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