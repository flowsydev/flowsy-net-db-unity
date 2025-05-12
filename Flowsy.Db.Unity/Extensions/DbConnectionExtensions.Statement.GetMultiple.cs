using System.Data;
using Dapper;
using Flowsy.Db.Unity.Conventions;

namespace Flowsy.Db.Unity.Extensions;

public static partial class DbConnectionExtensions
{   
    public static SqlMapper.GridReader GetMultipleFromStatement(
        this IDbConnection connection,
        string commandText,
        dynamic? parameters = null, 
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionHandler<SqlMapper.GridReader>? onExecuted = null
        )
    {
        var finalConventions = conventions ?? DbConventionSet.Default;
        var commandDefinition = finalConventions.Commands.BuildDefinition(commandText, parameters as object, transaction);
        
        onExecuting?.Invoke(commandDefinition);
        var result = connection.QueryMultiple(commandDefinition);
        return onExecuted is not null ? onExecuted(commandDefinition, result) : result;
    }
    
    public static async Task<SqlMapper.GridReader> GetMultipleFromStatementAsync(
        this IDbConnection connection,
        string commandText,
        dynamic? parameters = null, 
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        DbCommandPreExecutionHandler? onExecuting = null,
        DbCommandPostExecutionAsyncHandler<SqlMapper.GridReader>? onExecuted = null,
        CancellationToken cancellationToken = default
        )
    {
        var finalConventions = conventions ?? DbConventionSet.Default;
        var commandDefinition = finalConventions.Commands.BuildDefinition(commandText, parameters as object, transaction, cancellationToken);
        
        onExecuting?.Invoke(commandDefinition);
        var result = await connection.QueryMultipleAsync(commandDefinition);
        return onExecuted is not null ? await onExecuted(commandDefinition, result) : result;
    }
}