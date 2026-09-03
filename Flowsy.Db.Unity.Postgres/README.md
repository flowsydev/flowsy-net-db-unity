# Flowsy.Db.Unity.Postgres

Opt-in PostgreSQL integration for `Flowsy.Db.Unity`. It creates reusable `NpgsqlDataSource` instances and supports PostgreSQL enum and composite mappings while application services continue to depend on provider-neutral contracts.

## Install

```shell
dotnet add package Flowsy.Db.Unity.Postgres
```

The compatible `Flowsy.Db.Unity` package is included as a dependency.

## Configure

```csharp
using Flowsy.Db.Unity;
using Flowsy.Db.Unity.Postgres;

services.AddDatabases(options =>
{
    options
        .UsePostgres(
            "Catalog",
            connectionString,
            postgres => postgres.MapComposite<PostalAddress>("postal_address"))
        .AsDefault()
        .WithConventions()
        .ForEnums(
            memberNameCaseStyle: DbCaseStyle.LowerSnakeCase,
            mappings: [new DbEnumMapping<ProductStatus>("product_status", null)])
        .ForParameters(prefix: "p_", useNamedParameters: true)
        .WithDefault(DbCaseStyle.LowerSnakeCase);
});
```

Enum conventions are the source of truth for enum mappings. Use `MapComposite<T>` only for PostgreSQL composite types.

## Use

Application code remains provider-neutral:

```csharp
await using var db = await connectionHub.CreateSessionAsync("Catalog", cancellationToken);

var products = await db.QueryAsync<Product>(
    "select product_id, name, status from catalog.product",
    cancellationToken: cancellationToken);
```

For exceptional native operations, use `WithConnectionAsync<NpgsqlConnection>` so the session retains lifecycle and transaction control.

See the [complete usage guide](https://github.com/flowsydev/flowsy-net-db-unity/tree/main/Docs/Usage) and [package changelog](https://github.com/flowsydev/flowsy-net-db-unity/blob/main/Flowsy.Db.Unity.Postgres/CHANGELOG.md).
