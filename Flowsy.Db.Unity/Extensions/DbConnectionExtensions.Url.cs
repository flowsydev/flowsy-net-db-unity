using System.Data;
using System.Data.Common;
using Dapper;
using Flowsy.Db.Unity.Conventions;

namespace Flowsy.Db.Unity.Extensions;

public static partial class DbConnectionExtensions
{
    /// <summary>
    /// Gets a representation of the database connection as a URL.
    /// </summary>
    /// <param name="connection">
    /// The database connection to convert to a URL.
    /// </param>
    /// <returns>
    /// A string representing the database connection in URL format.
    /// </returns>
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