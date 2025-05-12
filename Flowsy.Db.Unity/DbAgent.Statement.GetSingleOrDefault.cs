using Flowsy.Db.Unity.Extensions;

namespace Flowsy.Db.Unity;

public partial class DbAgent
{
    public T? GetSingleOrDefaultFromStatement<T>(string commandText, dynamic? parameters = null)
        => Connection.GetSingleOrDefaultFromStatement<T>(
            commandText,
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

    public Task<T?> GetSingleOrDefaultFromStatementAsync<T>(string commandText, dynamic? parameters = null, CancellationToken cancellationToken = default)
        => Connection.GetSingleOrDefaultFromStatementAsync<T>(
            commandText,
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