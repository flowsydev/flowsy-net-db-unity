# Stored Procedures and Functions

Configure the default routine type and naming conventions per connection:

```csharp
.WithConventions()
.ForRoutines(DbRoutineType.StoredFunction)
.ForParameters(prefix: "p_", useNamedParameters: true)
```

Call routines through the specialized session methods:

```csharp
var product = await db.QuerySingleFromRoutineAsync<Product>(
    "store.product_get_by_id",
    new { ProductId = productId },
    cancellationToken
);

await db.ExecuteRoutineAsync(
    "store.product_update_price",
    new { ProductId = productId, Price = newPrice },
    cancellationToken
);
```

An overload accepting `DbRoutineType` can override the configured default for a specific call.
