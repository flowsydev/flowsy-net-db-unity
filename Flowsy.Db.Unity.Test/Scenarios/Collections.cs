using Flowsy.Db.Unity.Test.Infrastructure.Testing.Ordering;

namespace Flowsy.Db.Unity.Test.Scenarios;

public static class Collections
{
    public const string PostgresProductCategories = "Postgres product categories";
    public const string PostgresProducts = "Postgres products";
    public const string MySql = "MySql";
}

[CollectionDefinition(Collections.PostgresProductCategories), Order(1)]
public sealed class PostgresProductCategoriesCollection;

[CollectionDefinition(Collections.PostgresProducts), Order(2)]
public sealed class PostgresProductsCollection;

[CollectionDefinition(Collections.MySql), Order(3)]
public sealed class MySqlCollection;
