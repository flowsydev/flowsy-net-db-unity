using Dapper;
using Flowsy.Db.Unity.Extensions;

namespace Flowsy.Db.Unity;

public partial class DbAgent
{
    public SqlMapper.GridReader GetMultipleFromStatement(string commandText, dynamic? parameters = null)
        => Connection.GetMultipleFromStatement(
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

    public Task<SqlMapper.GridReader> GetMultipleFromStatementAsync(string commandText, dynamic? parameters = null, CancellationToken cancellationToken = default)
        => Connection.GetMultipleFromStatementAsync(
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