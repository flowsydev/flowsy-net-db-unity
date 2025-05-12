using Flowsy.Db.Unity.Extensions;

namespace Flowsy.Db.Unity;

public partial class DbAgent
{
    public T GetSingleFromRoutine<T>(string routineName, dynamic? parameters = null)
        => GetSingleFromRoutine<T>(routineName, null, parameters as object);

    public T GetSingleFromRoutine<T>(string routineName, DbRoutineType? routineType, dynamic? parameters = null)
        => Connection.GetSingleFromRoutine<T>(
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

    public Task<T> GetSingleFromRoutineAsync<T>(string routineName, dynamic? parameters = null, CancellationToken cancellationToken = default)
        => GetSingleFromRoutineAsync<T>(routineName, null, parameters as object, cancellationToken);

    public Task<T> GetSingleFromRoutineAsync<T>(string routineName, DbRoutineType? routineType, dynamic? parameters = null, CancellationToken cancellationToken = default)
        => Connection.GetSingleFromRoutineAsync<T>(
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