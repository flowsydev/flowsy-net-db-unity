# Flowsy.Db.Unity

Provider-neutral SQL data access for .NET 8 and .NET 10, built on Dapper and Evolve. It provides keyed connections, scoped sessions, naming conventions, routines, transactions, migrations, logging, tracing, and metrics without coupling application code to a specific database driver.

## Install

```shell
dotnet add package Flowsy.Db.Unity
```

Install a provider driver separately. For PostgreSQL, prefer the companion package:

```shell
dotnet add package Flowsy.Db.Unity.Postgres
```

## Quick Start

Register a connection once during application startup. Connection keys and anonymous-object properties use `PascalCase`; conventions translate them for the database.

```csharp
using Flowsy.Db.Unity;
using Flowsy.Db.Unity.Postgres;

services.AddDatabases(options =>
{
    options
        .UsePostgres("Catalog", connectionString)
        .AsDefault()
        .WithConventions()
        .ForParameters(prefix: "p_", useNamedParameters: true)
        .WithDefault(DbCaseStyle.LowerSnakeCase);
});
```

Inject the scoped `IDbConnectionHub`, create a session, and dispose the session after use:

```csharp
public sealed class ProductReader(IDbConnectionHub connectionHub)
{
    public async Task<Product?> FindAsync(Guid productId, CancellationToken cancellationToken)
    {
        await using var db = await connectionHub.CreateSessionAsync(cancellationToken);

        return await db.QuerySingleOrDefaultAsync<Product>(
            "select product_id, name, price from catalog.product where product_id = @p_product_id",
            new { ProductId = productId },
            cancellationToken);
    }
}
```

`QueryFirst*` requires at least one row. `QuerySingle*` rejects more than one row. The `OrDefault` variants allow an empty result.

## Transactions

Use the callback helper to commit on success and roll back on failure:

```csharp
await db.InTransactionAsync(async (session, ct) =>
{
    await session.ExecuteAsync(
        "update inventory set available = available - @p_quantity where product_id = @p_product_id",
        new { ProductId = productId, Quantity = quantity },
        ct);
}, cancellationToken);
```

Connections configured with `RequireTransactionForWrites` reject detected writes outside a transaction. Exceptions can be configured for statements that are safe in your environment.

## Common Operations

- `QueryAsync<T>`, `QueryFirst*Async<T>`, and `QuerySingle*Async<T>` read rows.
- `ExecuteAsync` and `ExecuteRoutineAsync` run commands and routines.
- `QueryMultipleAsync` processes multiple result sets; callback overloads keep the reader lifetime safe.
- `StreamAsync<T>` returns rows progressively as `IAsyncEnumerable<T>`.
- `WithSettingsAsync` applies scoped session settings and restores them afterward.
- `WithConnectionAsync` provides controlled access to the native connection for exceptional provider-specific work.
- `ExecuteScriptAsync` and `MigrateAsync` run scripts and Evolve migrations.

Per-call `DbSessionCallOptions` can override timeout, command type, and Dapper flags without changing the connection configuration.

## Conventions and Mapping

`DbConventionSet` controls routine, parameter, enum, date-time, and command behavior. Register result types with `MapTypes` when database column names use a different case style. Parameter mappings and enum member mappings provide explicit exceptions when a general convention is not enough. Built-in Dapper handlers support `DateOnly` and `TimeOnly`.

## Observability

Operations emit structured logs with session and operation identifiers. `DbDiagnostics` exposes `ActivitySource` traces and `Meter` metrics for command duration, errors, opened connections, and slow operations.

## Learn More

- [Complete Usage Guide](https://github.com/flowsydev/flowsy-net-db-unity/tree/main/Docs/Usage)
- [PostgreSQL Package](https://github.com/flowsydev/flowsy-net-db-unity/tree/main/Flowsy.Db.Unity.Postgres)
- [Changelog](https://github.com/flowsydev/flowsy-net-db-unity/blob/main/Flowsy.Db.Unity/CHANGELOG.md)
