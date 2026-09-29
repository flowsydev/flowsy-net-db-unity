# Stored Procedures And Functions

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

## Result Cardinality

`QueryFromRoutineAsync<T>` reads multiple rows. `QueryFirstFromRoutineAsync<T>` returns the first row and rejects an empty result; its `OrDefault` variant allows no rows. `QuerySingleFromRoutineAsync<T>` requires exactly one row, and `QuerySingleOrDefaultFromRoutineAsync<T>` allows zero or one row. Both `Single` variants reject multiple rows.

Use `QueryMultipleFromRoutineAsync` for several result sets and `StreamFromRoutineAsync<T>` for progressive enumeration. Keep the session alive until the reader or enumeration finishes.

## Provider SQL Formats

| Provider | Procedure | Scalar Function | Table Function |
|---|---|---|---|
| PostgreSQL | `CALL schema.name(...)` | `SELECT schema.name(...)` | `SELECT * FROM schema.name(...)` |
| SQL Server | `EXEC schema.name ...` | `SELECT schema.name(...)` | `SELECT * FROM schema.name(...)` |
| MySQL | `CALL name(...)` | `SELECT name(...)` | Unsupported |
| Oracle | `BEGIN name(...); END;` | `SELECT name(...) FROM DUAL` | `SELECT * FROM TABLE(name(...))` |
| IBM Db2 | `CALL schema.name(...)` | `VALUES schema.name(...)` | `SELECT * FROM TABLE(schema.name(...)) AS routine_result` |
| SQLite | Unsupported | `SELECT name(...)` | Unsupported |

The query wrappers request table results. Named routine arguments use `p_id => @p_id` for PostgreSQL and Db2, and `p_id = @p_id` for SQL Server. Other providers use their normal parameter placeholders. Set `useNamedParameters: false` for positional routine arguments.

For Db2, register an ADO.NET provider factory with `WithProvider(DbProviderFamily.Db2, invariantName, factory)`. Routine SQL follows the Db2 LUW dialect; driver configuration and engine-specific types remain the application's responsibility. The core package does not add an IBM driver or Db2 migrations to Evolve.

See IBM's [CALL documentation](https://www.ibm.com/docs/en/db2/11.5.x?topic=statements-call) and [table function reference](https://public.dhe.ibm.com/ps/products/db2/info/vr121/pdf/en_US/db2_dev_routines_121.pdf).
