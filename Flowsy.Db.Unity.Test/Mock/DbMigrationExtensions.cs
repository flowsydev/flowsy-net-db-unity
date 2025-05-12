using System.Data;
using System.Data.Common;
using EvolveDb;

namespace Flowsy.Db.Unity.Test.Mock;

public static class DbMigrationExtensions
{
    public static void Migrate(this IDbConnection connection, string scriptDirectoryPath, string? metadataTableName = null, string? metadataTableSchema = null)
    {
        if (connection is not DbConnection dbConnection)
            throw new ArgumentException("The connection must be a DbConnection.", nameof(connection));

        var evolve = new Evolve(dbConnection)
        {
            Locations = [scriptDirectoryPath],
            MetadataTableName = string.IsNullOrEmpty(metadataTableName) ? "changelog" : metadataTableName,
            MetadataTableSchema = string.IsNullOrEmpty(metadataTableSchema) ? string.Empty : metadataTableSchema,
        };
        
        evolve.Migrate();
    }
}