using Flowsy.Db.Unity.Extensions;

namespace Flowsy.Db.Unity;

public partial class DbAgent
{
    public T? GetSingleOrDefaultFromRoutine<T>(string routineName, dynamic? parameters = null)
        => GetSingleOrDefaultFromRoutine<T>(routineName, null, parameters as object);

    public T? GetSingleOrDefaultFromRoutine<T>(string routineName, DbRoutineType? routineType, dynamic? parameters = null)
        => Connection.GetSingleOrDefaultFromRoutine<T>(
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

    public Task<T?> GetSingleOrDefaultFromRoutineAsync<T>(string routineName, dynamic? parameters = null, CancellationToken cancellationToken = default)
        => GetSingleOrDefaultFromRoutineAsync<T>(routineName, null, parameters as object, cancellationToken);

    public Task<T?> GetSingleOrDefaultFromRoutineAsync<T>(string routineName, DbRoutineType? routineType, dynamic? parameters = null, CancellationToken cancellationToken = default)
        => Connection.GetSingleOrDefaultFromRoutineAsync<T>(
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