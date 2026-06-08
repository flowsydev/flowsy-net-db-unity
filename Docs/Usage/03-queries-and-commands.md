# Queries and Commands

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
