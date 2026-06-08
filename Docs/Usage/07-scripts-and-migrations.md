# Scripts and Migrations

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
