using Dapper;
using Flowsy.Db.Unity.Extensions;

namespace Flowsy.Db.Unity;

public partial class DbAgent
{
    public SqlMapper.GridReader GetMultipleFromRoutine(string routineName, dynamic? parameters = null)
        => GetMultipleFromRoutine(routineName, null, parameters as object);

    public SqlMapper.GridReader GetMultipleFromRoutine(string routineName, DbRoutineType? routineType, dynamic? parameters = null)
        => Connection.GetMultipleFromRoutine(
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

    public Task<SqlMapper.GridReader> GetMultipleFromRoutineAsync(string routineName, dynamic? parameters = null, CancellationToken cancellationToken = default)
        => GetMultipleFromRoutineAsync(routineName, null, parameters as object, cancellationToken);

    public Task<SqlMapper.GridReader> GetMultipleFromRoutineAsync(string routineName, DbRoutineType? routineType, dynamic? parameters = null, CancellationToken cancellationToken = default)
        => Connection.GetMultipleFromRoutineAsync(
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