using System.Data;
using Flowsy.Db.Unity.Conventions;

namespace Flowsy.Db.Unity.Extensions;

public static partial class DbConnectionExtensions
{
    public static int ExecuteScript(
        this IDbConnection connection,
        string scriptPath,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionHandler<int>? onExecuted = null
        )
        => connection.ExecuteScript(scriptPath, null, transaction, conventions, onExecuting, onExecuted);
    
    public static int ExecuteScript(
        this IDbConnection connection,
        string scriptPath,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionHandler<int>? onExecuted = null
        )
    {
        var scriptContent = File.ReadAllText(scriptPath);
        return string.IsNullOrEmpty(scriptContent) 
            ? 0
            : connection.ExecuteStatement(scriptContent, parameters as object, transaction, conventions, onExecuting, onExecuted);
    }

    public static Task<int> ExecuteScriptAsync(
        this IDbConnection connection,
        string scriptPath,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionAsyncHandler<int>? onExecuted = null,
        CancellationToken cancellationToken = default
    )
        => connection.ExecuteScriptAsync(scriptPath, null, transaction, conventions, onExecuting, onExecuted, cancellationToken);
    
    public static async Task<int> ExecuteScriptAsync(
        this IDbConnection connection,
        string scriptPath,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionAsyncHandler<int>? onExecuted = null,
        CancellationToken cancellationToken = default
        )
    {
        var scriptContent = await File.ReadAllTextAsync(scriptPath, cancellationToken);
        return string.IsNullOrEmpty(scriptContent) 
            ? 0
            : await connection.ExecuteStatementAsync(scriptContent, parameters as object, transaction, conventions, onExecuting, onExecuted, cancellationToken);
    }
}