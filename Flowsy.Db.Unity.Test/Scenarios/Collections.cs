using Xunit.Extensions.Ordering;

namespace Flowsy.Db.Unity.Test.Scenarios;

public static class Collections
{
    public const string Postgres = "Postgres";
    public const string MySql = "MySql";
}

[CollectionDefinition(Collections.Postgres), Order(1)]
public sealed class PostgresCollection;

[CollectionDefinition(Collections.MySql), Order(2)]
public sealed class MySqlCollection;