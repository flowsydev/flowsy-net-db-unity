using System.Data;
using Flowsy.Db.Unity.Conventions;

namespace Flowsy.Db.Unity.Extensions;

public static partial class DbConnectionExtensions
{
    /// <summary>
    /// Executes a SQL script from a file.
    /// </summary>
    /// <param name="connection">
    /// The database connection to use for executing the script.
    /// </param>
    /// <param name="scriptPath">
    /// The path to the SQL script file to execute.
    /// </param>
    /// <param name="transaction">
    /// An optional transaction to use for the script execution.
    /// </param>
    /// <param name="conventions">
    /// The conventions to use for the script execution. If null, the default conventions will be used.
    /// </param>
    /// <param name="onExecuting">
    /// An optional action to execute before the script is executed. This can be used from services to raise events or log the command.
    /// </param>
    /// <param name="onExecuted">
    /// An optional action to execute after the script is executed. This can be used from services to raise events or log the command.
    /// </param>
    /// <returns>
    /// The number of rows affected by the script execution.
    /// </returns>
    public static int ExecuteScript(
        this IDbConnection connection,
        string scriptPath,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionHandler<int>? onExecuted = null
        )
        => connection.ExecuteScript(scriptPath, null, transaction, conventions, onExecuting, onExecuted);
    
    /// <summary>
    /// Executes a SQL script from a file.
    /// </summary>
    /// <param name="connection">
    /// The database connection to use for executing the script.
    /// </param>
    /// <param name="scriptPath">
    /// The path to the SQL script file to execute.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the SQL script. This can be an anonymous object or a dictionary.
    /// </param>
    /// <param name="transaction">
    /// An optional transaction to use for the script execution.
    /// </param>
    /// <param name="conventions">
    /// The conventions to use for the script execution. If null, the default conventions will be used.
    /// </param>
    /// <param name="onExecuting">
    /// An optional action to execute before the script is executed. This can be used from services to raise events or log the command.
    /// </param>
    /// <param name="onExecuted">
    /// An optional action to execute after the script is executed. This can be used from services to raise events or log the command.
    /// </param>
    /// <returns>
    /// The number of rows affected by the script execution.
    /// </returns>
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
        using var scriptStream = File.Open(scriptPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return connection.ExecuteScript(scriptStream, parameters as object, transaction, conventions, onExecuting, onExecuted);
    }

    /// <summary>
    /// Executes a SQL script from a stream.
    /// </summary>
    /// <param name="connection">
    /// The database connection to use for executing the script.
    /// </param>
    /// <param name="scriptStream">
    /// The stream containing the SQL script to execute.
    /// </param>
    /// <param name="transaction">
    /// An optional transaction to use for the script execution.
    /// </param>
    /// <param name="conventions">
    /// The conventions to use for the script execution. If null, the default conventions will be used.
    /// </param>
    /// <param name="onExecuting">
    /// An optional action to execute before the script is executed. This can be used from services to raise events or log the command.
    /// </param>
    /// <param name="onExecuted">
    /// An optional action to execute after the script is executed. This can be used from services to raise events or log the command.
    /// </param>
    /// <returns>
    /// The number of rows affected by the script execution.
    /// </returns>
    public static int ExecuteScript(
        this IDbConnection connection,
        Stream scriptStream,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionHandler<int>? onExecuted = null
        )
        => connection.ExecuteScript(scriptStream, null, transaction, conventions, onExecuting, onExecuted);

    /// <summary>
    /// Executes a SQL script from a stream.
    /// </summary>
    /// <param name="connection">
    /// The database connection to use for executing the script.
    /// </param>
    /// <param name="scriptStream">
    /// The stream containing the SQL script to execute.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the SQL script. This can be an anonymous object or a dictionary.
    /// </param>
    /// <param name="transaction">
    /// An optional transaction to use for the script execution.
    /// </param>
    /// <param name="conventions">
    /// The conventions to use for the script execution. If null, the default conventions will be used.
    /// </param>
    /// <param name="onExecuting">
    /// An optional action to execute before the script is executed. This can be used from services to raise events or log the command.
    /// </param>
    /// <param name="onExecuted">
    /// An optional action to execute after the script is executed. This can be used from services to raise events or log the command.
    /// </param>
    /// <returns>
    /// The number of rows affected by the script execution.
    /// </returns>
    public static int ExecuteScript(
        this IDbConnection connection,
        Stream scriptStream,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionHandler<int>? onExecuted = null
        )
    {
        using var reader = new StreamReader(scriptStream);
        var scriptContent = reader.ReadToEnd();
        
        return string.IsNullOrEmpty(scriptContent) 
            ? 0
            : connection.ExecuteStatement(scriptContent, parameters as object, transaction, conventions, onExecuting, onExecuted);
    }

    /// <summary>
    /// Asynchronously executes a SQL script from a file.
    /// </summary>
    /// <param name="connection">
    /// The database connection to use for executing the script.
    /// </param>
    /// <param name="scriptPath">
    /// The path to the SQL script file to execute.
    /// </param>
    /// <param name="transaction">
    /// An optional transaction to use for the script execution.
    /// </param>
    /// <param name="conventions">
    /// The conventions to use for the script execution. If null, the default conventions will be used.
    /// </param>
    /// <param name="onExecuting">
    /// An optional action to execute before the script is executed. This can be used from services to raise events or log the command.
    /// </param>
    /// <param name="onExecuted">
    /// An optional action to execute after the script is executed. This can be used from services to raise events or log the command.
    /// </param>
    /// <param name="cancellationToken">
    /// An optional cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// The number of rows affected by the script execution.
    /// </returns>
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

    /// <summary>
    /// Asynchronously executes a SQL script from a file.
    /// </summary>
    /// <param name="connection">
    /// The database connection to use for executing the script.
    /// </param>
    /// <param name="scriptPath">
    /// The path to the SQL script file to execute.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the SQL script. This can be an anonymous object or a dictionary.
    /// </param>
    /// <param name="transaction">
    /// An optional transaction to use for the script execution.
    /// </param>
    /// <param name="conventions">
    /// The conventions to use for the script execution. If null, the default conventions will be used.
    /// </param>
    /// <param name="onExecuting">
    /// An optional action to execute before the script is executed. This can be used from services to raise events or log the command.
    /// </param>
    /// <param name="onExecuted">
    /// An optional action to execute after the script is executed. This can be used from services to raise events or log the command.
    /// </param>
    /// <param name="cancellationToken">
    /// An optional cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// The number of rows affected by the script execution.
    /// </returns>
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
        await using var scriptStream = File.Open(scriptPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return await connection.ExecuteScriptAsync(scriptStream, parameters as object, transaction, conventions, onExecuting, onExecuted, cancellationToken);
    }
    

    /// <summary>
    /// Asynchronously executes a SQL script from a stream.
    /// </summary>
    /// <param name="connection">
    /// The database connection to use for executing the script.
    /// </param>
    /// <param name="scriptStream">
    /// The stream containing the SQL script to execute.
    /// </param>
    /// <param name="transaction">
    /// An optional transaction to use for the script execution.
    /// </param>
    /// <param name="conventions">
    /// The conventions to use for the script execution. If null, the default conventions will be used.
    /// </param>
    /// <param name="onExecuting">
    /// An optional action to execute before the script is executed. This can be used from services to raise events or log the command.
    /// </param>
    /// <param name="onExecuted">
    /// An optional action to execute after the script is executed. This can be used from services to raise events or log the command.
    /// </param>
    /// <param name="cancellationToken">
    /// An optional cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// The number of rows affected by the script execution.
    /// </returns>
    public static Task<int> ExecuteScriptAsync(
        this IDbConnection connection,
        Stream scriptStream,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionAsyncHandler<int>? onExecuted = null,
        CancellationToken cancellationToken = default
        )
        => connection.ExecuteScriptAsync(scriptStream, null, transaction, conventions, onExecuting, onExecuted, cancellationToken);

    /// <summary>
    /// Asynchronously executes a SQL script from a stream.
    /// </summary>
    /// <param name="connection">
    /// The database connection to use for executing the script.
    /// </param>
    /// <param name="scriptStream">
    /// The stream containing the SQL script to execute.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the SQL script. This can be an anonymous object or a dictionary.
    /// </param>
    /// <param name="transaction">
    /// An optional transaction to use for the script execution.
    /// </param>
    /// <param name="conventions">
    /// The conventions to use for the script execution. If null, the default conventions will be used.
    /// </param>
    /// <param name="onExecuting">
    /// An optional action to execute before the script is executed. This can be used from services to raise events or log the command.
    /// </param>
    /// <param name="onExecuted">
    /// An optional action to execute after the script is executed. This can be used from services to raise events or log the command.
    /// </param>
    /// <param name="cancellationToken">
    /// An optional cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// The number of rows affected by the script execution.
    /// </returns>
    public static async Task<int> ExecuteScriptAsync(
        this IDbConnection connection,
        Stream scriptStream,
        dynamic? parameters = null,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionAsyncHandler<int>? onExecuted = null,
        CancellationToken cancellationToken = default
        )
    {
        using var reader = new StreamReader(scriptStream);
        var scriptContent = await reader.ReadToEndAsync();
        
        return string.IsNullOrEmpty(scriptContent) 
            ? 0
            : await connection.ExecuteStatementAsync(scriptContent, parameters as object, transaction, conventions, onExecuting, onExecuted, cancellationToken);
    }
}