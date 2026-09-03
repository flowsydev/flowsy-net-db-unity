# Transactions, Observability, And Guardrails

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

Use the callback helper when the session should manage commit and rollback:

```csharp
await db.InTransactionAsync(async (session, token) =>
{
    await session.ExecuteAsync(firstStatement, firstParameters, token);
    await session.ExecuteAsync(secondStatement, secondParameters, token);
}, cancellationToken);
```

`InExistingOrNewTransactionAsync` joins an existing transaction and only completes one it creates.

## Write Guard

An opt-in guard can require an active transaction for detected write statements and routine execution:

```csharp
.WithWriteTransactionGuard(
    enabled: true,
    exceptions: ["VACUUM"])
```

The default detector is deliberately conservative and replaceable through `IDbWriteOperationDetector`; it is not a complete SQL parser.

## Session Settings

Apply allowlisted connection settings for the duration of a callback:

```csharp
await db.WithSettingsAsync(
    [new DbSessionSetting("statement_timeout", 30_000)],
    (session, token) => session.ExecuteAsync(statement, parameters, token),
    cancellationToken);
```

Applied settings are cleaned in reverse order, including when a later setting or the callback fails. Add non-default names explicitly with `AllowSessionSettings`.

Configure normal operation logging with `WithLogLevel`. Log entries include `SessionId` and `OperationId` for correlation, while failures are logged as errors.

`DbDiagnostics.ActivitySource` and `DbDiagnostics.Meter` expose standard tracing and metrics without recording SQL or parameter values. Use `WithSlowOperationThreshold` to emit warnings for slow operations.
