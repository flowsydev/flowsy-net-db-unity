using Flowsy.Db.Unity.Extensions;

namespace Flowsy.Db.Unity;

public partial class DbAgent
{
    public int ExecuteScript(string scriptPath, dynamic? parameters = null)
        => Connection.ExecuteScript(
            scriptPath,
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
    
    public Task<int> ExecuteScriptAsync(string scriptPath, CancellationToken cancellationToken = default)
        => ExecuteScriptAsync(scriptPath, null, cancellationToken);
    
    public Task<int> ExecuteScriptAsync(string scriptPath, dynamic? parameters = null, CancellationToken cancellationToken = default)
        => Connection.ExecuteScriptAsync(
            scriptPath,
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