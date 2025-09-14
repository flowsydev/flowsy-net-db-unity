namespace Flowsy.Db.Unity.Test.Mock.Infrastructure.Database;

public class DbConnections
{
    public const string MySql = "MySql";
    public const string Oracle = "Oracle";
    public const string Postgres = "Postgres";
    public const string Sqlite = "Sqlite";
    public const string SqlServer = "SqlServer";

    public static readonly string[] All = [MySql, Oracle, Postgres, Sqlite, SqlServer];
}