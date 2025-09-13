using System.Data;
using Flowsy.Db.Unity.Resources;
using Microsoft.Extensions.Logging;

namespace Flowsy.Db.Unity;

public partial class DbSession
{
    /// <summary>
    /// Executes a SQL script from a file.
    /// </summary>
    /// <param name="scriptPath">
    /// Path of the file that contains the SQL script to execute.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <exception cref="FileNotFoundException">
    /// Thrown if the specified file does not exist.
    /// </exception>
    protected async Task ExecuteScriptFileAsync(string scriptPath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(scriptPath))
            throw new FileNotFoundException(string.Format(Strings.ScriptXNotFound, scriptPath), scriptPath);

        await using var stream = File.OpenRead(scriptPath);
        await ExecuteScriptAsync(stream, scriptPath, cancellationToken);
    }
    
    /// <summary>
    /// Executes a SQL script from a file or directory.
    /// If the `scriptPath` parameter is a directory, all files with `.sql` extension will be executed in alphabetical order.
    /// </summary>
    /// <param name="scriptPath">
    /// Path of the file or directory that contains the SQL script to execute.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of executing the SQL script.
    /// </returns>
    public async Task ExecuteScriptAsync(string scriptPath, CancellationToken cancellationToken = default)
    {
        var attr = File.GetAttributes(scriptPath);
        if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
        {
            var files = Directory
                .EnumerateFiles(scriptPath, "*.sql", SearchOption.AllDirectories)
                .OrderBy(f => f);
            
            foreach (var file in files)
                await ExecuteScriptFileAsync(file, cancellationToken);

            return;
        }
        
        await ExecuteScriptFileAsync(scriptPath, cancellationToken);
    }

    /// <summary>
    /// Executes a SQL script from a data stream.
    /// </summary>
    /// <param name="scriptStream">
    /// Data stream that contains the SQL script to execute.
    /// </param>
    /// <param name="filePath">
    /// File path, if applicable, from which the script was loaded, used for logging purposes.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of executing the SQL script from the data stream.
    /// </returns>
    public async Task ExecuteScriptAsync(Stream scriptStream, string? filePath = null, CancellationToken cancellationToken = default)
    {
        if (scriptStream is {CanSeek: true, Position: > 0})
        {
            scriptStream.Seek(0, SeekOrigin.Begin);
        }

        var operationId = CreateOperationId();

        string? scriptContent;
        {
            using var reader = new StreamReader(scriptStream);
            scriptContent = await reader.ReadToEndAsync();
            if (string.IsNullOrWhiteSpace(scriptContent))
            {
                _logger?.LogWarning(
                    "[ SESSION:{SessionId} > OP:{OperationId} ] Skipping empty script: {FilePath}",
                    SessionId,
                    operationId,
                    string.IsNullOrEmpty(filePath) ? "Unknown file path" : filePath
                );
                return;
            }
        }
        
        _logger?.Log(
            Configuration.LogLevel,
            "[ SESSION:{SessionId} > OP:{OperationId} ] Executing script: {FilePath}",
            SessionId,
            operationId,
            string.IsNullOrEmpty(filePath) ? "unknown file path" : filePath
        );

        try
        {
            await ExecuteCommandAsync(scriptContent, CommandType.Text, cancellationToken);

            _logger?.Log(
                Configuration.LogLevel,
                "[ SESSION:{SessionId} > OP:{OperationId} ] Script executed: {FilePath}",
                SessionId,
                operationId,
                string.IsNullOrEmpty(filePath) ? "unknown file path" : filePath
            );
        }
        catch (Exception exception)
        {
            _logger?.LogError(
                exception,
                "[ SESSION:{SessionId} > OP:{OperationId} ] Error executing script: {FilePath}",
                SessionId,
                operationId,
                string.IsNullOrEmpty(filePath) ? "unknown file path" : filePath
            );
            throw;
        }
    }
}