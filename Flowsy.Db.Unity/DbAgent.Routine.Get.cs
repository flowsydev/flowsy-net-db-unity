using Flowsy.Db.Unity.Extensions;

namespace Flowsy.Db.Unity;

public partial class DbAgent
{
    public IEnumerable<T> GetFromRoutine<T>(string routineName, dynamic? parameters = null)
        => GetFromRoutine<T>(routineName, null, parameters as object);

    public IEnumerable<T> GetFromRoutine<T>(string routineName, DbRoutineType? routineType, dynamic? parameters = null)
        => Connection.GetFromRoutine<T>(
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

    public Task<IEnumerable<T>> GetFromRoutineAsync<T>(
        string routineName,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
    )
        => GetFromRoutineAsync<T>(routineName, null, parameters as object, cancellationToken);

    public Task<IEnumerable<T>> GetFromRoutineAsync<T>(
        string routineName,
        DbRoutineType? routineType,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
    )
        => Connection.GetFromRoutineAsync<T>(
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