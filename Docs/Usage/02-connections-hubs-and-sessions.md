# Connections, Hubs, And Sessions

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

## Session Concurrency

A session owns one connection and its optional transaction. Do not use the same session concurrently from multiple threads or overlapping asynchronous operations. Create an exclusive session for parallel work.

## Native Connection Access

Use `WithConnectionAsync` when an infrastructure operation needs the session's current native connection and transaction:

```csharp
await db.WithConnectionAsync<NpgsqlConnection>(async (connection, transaction, token) =>
{
    // Use a provider-native API without taking ownership of either resource.
    await Task.CompletedTask;
}, cancellationToken);
```

The callback must not close, dispose, or complete session-owned resources.
