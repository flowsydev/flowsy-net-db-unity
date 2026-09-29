# Scripts And Migrations

Execute an individual SQL file or all supported files in a directory:

```csharp
await db.ExecuteScriptAsync("database/bootstrap.sql", cancellationToken);
await db.ExecuteScriptAsync("database/reference-data", cancellationToken);
```

Configure Evolve migrations per connection:

```csharp
.WithMigrations("database/migrations")
```

Then run them through an exclusive session:

```csharp
await using var db = await hub.CreateSessionAsync(
    "Store",
    DbConnectionUsage.Exclusive,
    cancellationToken: cancellationToken
);

await db.MigrateAsync(cancellationToken);
```

## Scripts from Streams

Execute a readable stream and supply a filename for logging:

```csharp
await using var stream = File.OpenRead("database/bootstrap.sql");
await db.ExecuteScriptAsync(stream, filePath: "database/bootstrap.sql", cancellationToken: cancellationToken);
```

Directory execution includes `.sql` files recursively, ordered by path. Empty scripts are skipped with a warning. Stream execution consumes and closes the supplied stream.

## Migration Configuration

Configure optional setup and finalization scripts, metadata location, and out-of-order behavior:

```csharp
.WithMigrations(
    migrationScriptPath: "database/migrations",
    preMigrationScript: "database/before.sql",
    postMigrationScript: "database/after.sql",
    historyTableName: "changelog",
    historySchemaName: "public",
    outOfOrder: false)
```

`MigrateAsync` validates the configuration and migration directory, executes the optional pre-migration script in a transaction, applies Evolve migrations through a separate connection, and executes the optional post-migration script in another transaction. These steps do not form one atomic transaction.

Use Evolve's versioned and repeatable naming conventions, such as `Versioned/V001__create_product_table.sql` and `Repeatable/R__fn_product_get_all.sql`. Database support for migrations is determined by [Evolve](https://evolve-db.netlify.app/).
