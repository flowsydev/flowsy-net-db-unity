using Flowsy.Db.Unity.Extensions;

namespace Flowsy.Db.Unity;

public partial class DbAgent
{
    public T? GetFirstOrDefaultFromRoutine<T>(string routineName, dynamic? parameters = null)
        => GetFirstOrDefaultFromRoutine<T>(routineName, null, parameters as object);

    public T? GetFirstOrDefaultFromRoutine<T>(string routineName, DbRoutineType? routineType, dynamic? parameters = null)
        => Connection.GetFirstOrDefaultFromRoutine<T>(
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

    public Task<T?> GetFirstOrDefaultFromRoutineAsync<T>(string routineName, dynamic? parameters = null, CancellationToken cancellationToken = default)
        => GetFirstOrDefaultFromRoutineAsync<T>(routineName, null, parameters as object, cancellationToken);

    public Task<T?> GetFirstOrDefaultFromRoutineAsync<T>(string routineName, DbRoutineType? routineType, dynamic? parameters = null, CancellationToken cancellationToken = default)
        => Connection.GetFirstOrDefaultFromRoutineAsync<T>(
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