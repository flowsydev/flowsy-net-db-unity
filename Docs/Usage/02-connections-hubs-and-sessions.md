# Connections, Hubs, and Sessions

Use `IDbConnectionHub` to create sessions and manage connection lifetimes:

```csharp
await using var db = await hub.CreateSessionAsync("Store", cancellationToken);
```

A session exposes its connection key, configuration, usage mode, session identifier, query operations, routines, scripts, migrations, and transactions.

Shared connections are reused within the hub scope. Exclusive connections are useful for long-running operations, migrations, or parallel work:

```csharp
await using var db = await hub.CreateSessionAsync(
    "Store",
    DbConnectionUsage.Exclusive,
    open: true,
    cancellationToken
);
```

Prefer `await using` so sessions and exclusive connections are released asynchronously.
