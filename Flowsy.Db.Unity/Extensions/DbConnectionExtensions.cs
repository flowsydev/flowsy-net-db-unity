using System.Data;
using System.Data.Common;

namespace Flowsy.Db.Unity.Extensions;

/// <summary>
/// Provides extension methods for database connections.
/// </summary>
public static class DbConnectionExtensions
{
    /// <summary>
    /// Gets the database URL from a database connection.
    /// </summary>
    /// <param name="connection">The database connection to extract URL information from.</param>
    /// <returns>A string representing the database URL in the format: ConnectionType://user@host:port/database</returns>
    public static string GetDatabaseUrl(this IDbConnection connection)
    {
        var connectionStringBuilder = new DbConnectionStringBuilder
        {
            ConnectionString = connection.ConnectionString
        };
        
        // Build string like NpgsqlConnection://user@host:port/database
        var connectionType = connection.GetType();
        string[] userKeys = ["User Id", "UserID", "Username", "UserName"];
        string[] hostKeys = ["Host", "Data Source", "Server"];
        string[] portKeys = ["Port", "PortNumber"];
        string[] databaseKeys = ["Database", "Initial Catalog", "DataBase", "Catalog"];

        var user = "unknown_user";
        var host = "unknown_host";
        var port = string.Empty;
        var database = string.Empty;
        foreach (var key in userKeys)
        {
            if (!connectionStringBuilder.ContainsKey(key)) continue;
            user = connectionStringBuilder[key].ToString() ?? string.Empty;
            break;
        }
        foreach (var key in hostKeys)
        {
            if (!connectionStringBuilder.ContainsKey(key)) continue;
            host = connectionStringBuilder[key].ToString() ?? string.Empty;
            break;
        }
        foreach (var key in portKeys)
        {
            if (!connectionStringBuilder.ContainsKey(key)) continue;
            port = connectionStringBuilder[key].ToString() ?? string.Empty;
            break;
        }
        foreach (var key in databaseKeys)
        {
            if (!connectionStringBuilder.ContainsKey(key)) continue;
            database = connectionStringBuilder[key].ToString() ?? string.Empty;
            break;
        }
        
        var url = $"{connectionType.Name}://{user}@{host}";
        
        if (!string.IsNullOrEmpty(port))
            url += $":{port}";
        
        if (!string.IsNullOrEmpty(database))
            url += $"/{database}";
        
        return url;
    }
}