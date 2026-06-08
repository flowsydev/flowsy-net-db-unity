# Transactions and Logging

Transactions belong to a session:

```csharp
await db.BeginTransactionAsync(cancellationToken);

try
{
    await db.ExecuteAsync(statement, parameters, cancellationToken);
    await db.CommitTransactionAsync(cancellationToken);
}
catch
{
    await db.RollbackTransactionAsync(cancellationToken);
    throw;
}
```

The default isolation level is `ReadCommitted`; overloads accept another `IsolationLevel`. Disposing a session with an active transaction rolls it back.

Configure normal operation logging with `WithLogLevel`. Log entries include `SessionId` and `OperationId` for correlation, while failures are logged as errors.
