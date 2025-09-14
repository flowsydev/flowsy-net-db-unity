namespace Flowsy.Db.Unity.Test.Mock.Infrastructure.Database;

public class DbConnections
{
    public const string Postgres = "Postgres";
    public const string MySql = "MySql";
    
    public static readonly string[] All = [Postgres, MySql];
}