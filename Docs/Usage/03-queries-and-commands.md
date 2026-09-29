# Queries And Commands

`IDbSession` wraps common Dapper operations and applies the configured conventions:

```csharp
var products = await db.QueryAsync<Product>(
    "select product_id, name, price from store.product where active = @p_active",
    new { Active = true },
    cancellationToken
);

var affected = await db.ExecuteAsync(
    "update store.product set price = @p_price where product_id = @p_product_id",
    new { ProductId = productId, Price = newPrice },
    cancellationToken
);
```

Use `QueryFirstAsync`, `QueryFirstOrDefaultAsync`, `QuerySingleAsync`, or `QuerySingleOrDefaultAsync` according to the expected result cardinality. `QueryMultipleAsync` handles commands that return multiple result sets.

Prefer the callback overload when all result sets can be consumed together. It disposes the reader even when the callback fails:

```csharp
var result = await db.QueryMultipleAsync(
    "select count(*) from product; select max(price) from product",
    async (reader, token) =>
    {
        var count = await reader.ReadSingleAsync<int>();
        var maximum = await reader.ReadSingleAsync<decimal>();
        return (count, maximum);
    },
    cancellationToken: cancellationToken);
```

## Streaming

Use `StreamAsync<T>` to process large result sets without buffering them all in memory:

```csharp
await foreach (var product in db.StreamAsync<Product>(sql, cancellationToken: cancellationToken))
    await index.WriteAsync(product, cancellationToken);
```

The session must remain alive until enumeration finishes.

## Per-Call Options

Command timeout, type, Dapper flags, and a sanitized telemetry tag can be overridden for one operation:

```csharp
var products = await db.QueryAsync<Product>(
    sql,
    parameters,
    new DbSessionCallOptions { Timeout = 90, Tag = "catalog-refresh" },
    cancellationToken);
```
