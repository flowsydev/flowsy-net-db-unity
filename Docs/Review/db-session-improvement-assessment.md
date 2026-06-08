# `DbSession` Improvement Assessment

Analysis date: June 8, 2026.

## Scope

This review evaluates potential improvements to `Flowsy.Db.Unity` based on common operational needs for database access libraries. The goal is to strengthen the existing multi-provider design without coupling the core package to a particular database engine or changing current behavior by default.

The assessment covers the current `IDbSession`, `DbSession`, `IDbConnectionFactory`, `IDbConnectionHub`, conventions, parameter mapping, transactions, logging, and extensibility APIs.

## Executive Summary

All twelve improvement areas evaluated in this document are relevant to `Flowsy.Db.Unity`, but they do not have the same urgency:

- **High-value additions:** per-call options, transaction helpers, safe `QueryMultiple` callbacks, and explicit thread-safety documentation.
- **Important lifecycle and performance additions:** connection scopes, streaming, and controlled access to the underlying connection.
- **Useful operational safeguards:** standard tracing and metrics, optional write guards, and validated session settings.
- **Provider extension opportunities:** advanced PostgreSQL helpers and provider-specific bulk operations.
- **Lower-priority ergonomics:** configurable Dapper type handlers and optional mapping attributes.

Several areas already have a useful foundation:

- `DbCommandConvention` supports timeout and Dapper command flags at connection-configuration level.
- `IDbConnectionHub` supports shared and exclusive connection usage.
- `DbSession` provides manual transactions with rollback during disposal.
- `DbParameterBuilder` supports provider-aware parameters, arrays, nullable values, enums, and custom enum types.
- Provider capabilities and routine formatting are centralized instead of embedded in individual operations.
- Structured logs correlate sessions and operations.

## Current Strengths

- Multi-provider descriptors for PostgreSQL, SQL Server, MySQL, Oracle, SQLite, and generic providers.
- Configurable conventions for commands, routines, parameters, enums, date-time values, and object names.
- Provider-aware parameter creation through [`DbParameterBuilder`](../../Flowsy.Db.Unity/DbParameterBuilder.cs).
- Separate `partial` files for query, command, routine, script, migration, and transaction behavior.
- Shared and exclusive connection management through [`IDbConnectionHub`](../../Flowsy.Db.Unity/IDbConnectionHub.cs).
- Explicit synchronous and asynchronous transaction APIs in [`IDbSession`](../../Flowsy.Db.Unity/IDbSession.cs).
- Structured operation logging with session and operation identifiers.
- Extensible connection and session factories.

## Improvement Assessment

### 1. Per-Call Options

**Applicability:** Applies partially.

[`DbCommandConvention`](../../Flowsy.Db.Unity/Conventions/DbCommandConvention.cs) already represents timeout and Dapper command flags, and `DbSession.BuildCommandDefinition` can accept a command convention internally. Public session operations, however, always use the connection-level convention and do not expose per-call overrides.

Recommended direction:

- Add an immutable `DbSessionCallOptions` type.
- Support timeout, command flags, command type, buffering behavior, and an optional correlation tag.
- Add opt-in overloads for commands, queries, routines, and multiple-result operations.
- Fall back to configured command conventions when an option is not specified.
- Sanitize correlation tags before adding them to SQL comments or telemetry.

**Priority:** High.

### 2. Connection Scopes Without Transactions

**Applicability:** Applies partially.

`IDbConnectionHub` already supports shared and exclusive connections, and a session reuses its assigned connection. There is no explicit session-level scope that guarantees a connection remains open across several operations without starting a transaction.

Recommended direction:

- Add synchronous and asynchronous connection-scope helpers.
- Track nested scope depth and close only when the outermost scope completes, when the session owns the connection lifecycle.
- Treat an active transaction as a stronger connection scope.
- Document that sessions are scoped services and are not thread-safe.

**Priority:** High.

### 3. Transaction Helpers

**Applicability:** Applies.

Transactions currently require consumers to coordinate begin, commit, rollback, and exception handling manually.

Recommended direction:

- Add `InTransactionAsync` helpers with `Task` and `Task<T>` callbacks.
- Add variants that reuse an existing transaction or create a new one.
- Preserve all current manual transaction methods.
- Reuse the existing session-operation logging format for transaction start, success, rollback, and failure.

**Priority:** High.

### 4. Optional Write Guards

**Applicability:** Applies.

The library does not currently provide an opt-in rule requiring a transaction for write operations.

Recommended direction:

- Add a per-connection `RequireTransactionForWrites` option.
- Define an extensible `IDbWriteOperationDetector`.
- Provide a conservative default detector for common write commands.
- Allow explicit bypasses for migrations and administrative operations.

A detector should be documented as an operational guardrail, not as a complete SQL parser.

**Priority:** Medium-High.

### 5. Result Streaming

**Applicability:** Applies.

`QueryAsync<T>` returns `IEnumerable<T>`, and current public APIs do not expose `IAsyncEnumerable<T>` streaming.

Recommended direction:

- Add `QueryStreamAsync<T>` and `QueryStreamFromRoutineAsync<T>`.
- Use `DbCommand.ExecuteReaderAsync` when the provider connection derives from `DbConnection`.
- Keep connection, command, reader, and cancellation-token lifetimes tied to asynchronous enumeration.
- Define a fallback or explicit capability error for providers that cannot support the generic streaming path.

**Priority:** Medium-High.

### 6. Safe `QueryMultiple` Consumption

**Applicability:** Applies.

[`QueryMultipleAsync`](../../Flowsy.Db.Unity/DbSession.QueryMultiple.cs) and its routine equivalent return `SqlMapper.GridReader` directly. This is flexible, but consumers must dispose the reader and keep the session alive until all result sets are consumed.

Recommended direction:

- Preserve the current APIs for compatibility.
- Add callback overloads that consume and dispose the reader inside the session operation.
- Provide callbacks returning either `Task` or `Task<T>`.

**Priority:** High.

### 7. Standard Tracing and Metrics

**Applicability:** Applies.

The current structured logging provides useful correlation, but the library does not expose `ActivitySource`, `Meter`, command-duration metrics, error counters, or slow-query thresholds.

Recommended direction:

- Add a `Flowsy.Db.Unity` activity source and meter.
- Record stable tags such as database system, operation type, connection key, session identifier, and routine name.
- Record command duration, command count, failures, and connection-state metrics.
- Add a configurable slow-query threshold.
- Do not record parameter values or complete SQL statements by default.

**Priority:** Medium.

### 8. Validated Database Session Settings

**Applicability:** Applies.

Provider descriptors expose capabilities and default schemas, but sessions do not provide a safe, scoped mechanism for settings such as schema search paths, time zones, or lock and statement timeouts.

Recommended direction:

- Add neutral `DbSessionSetting` and `IDbSessionSettingFormatter` abstractions.
- Expose scoped synchronous and asynchronous APIs that restore previous values when possible.
- Implement provider-specific allowlists and strict value validation.
- Keep provider-specific setting names and SQL outside the generic core behavior.

**Priority:** Medium.

### 9. Advanced PostgreSQL Extensions

**Applicability:** Applies as an optional provider extension.

The core already supports PostgreSQL routines, named parameters, arrays, custom enum types, and provider-aware casts. Advanced features should build on these existing conventions instead of introducing a parallel command-building path.

Recommended direction:

- Add optional helpers for function calls, custom type hints, composite types, and validated array casts.
- Expose prepared SQL and parameter descriptors for focused testing.
- Keep PostgreSQL-only APIs outside the provider-neutral session interface.

**Priority:** Medium.

### 10. Controlled Access to the Underlying Connection

**Applicability:** Applies partially.

Consumers can request connections from `IDbConnectionHub`, but `IDbSession` does not expose a callback that safely reuses its current connection and active transaction.

Recommended direction:

- Add a generic `WithConnectionAsync<T>` callback receiving `IDbConnection`, the current optional transaction, and a cancellation token.
- Keep lifecycle ownership inside the session.
- Implement provider-specific extensions for native bulk operations and other specialized APIs.

**Priority:** Medium.

### 11. Dapper Type Handlers and Modern .NET Types

**Applicability:** Applies partially.

The provider descriptor maps common CLR types, and parameter conventions already handle `DateTimeOffset`, arrays, and enums. There is no first-class configuration point for Dapper `TypeHandler` registration, and explicit `DateOnly` and `TimeOnly` support should be evaluated.

Recommended direction:

- Add a service-configuration extension point for registering Dapper type handlers.
- Evaluate explicit provider mappings and tests for `DateOnly` and `TimeOnly`.
- Preserve parameter conventions as the default path.

**Priority:** Low-Medium.

### 12. Optional Parameter and Database-Type Attributes

**Applicability:** Applies, with caution.

`DbParameterBuilder` currently resolves parameter names and types from centralized conventions. Optional attributes could simplify exceptions for legacy schemas or custom types, but they would also introduce persistence concerns into consumer models.

Recommended direction:

- Consider neutral attributes such as `DbParameterName`, `DbTypeName`, and `DbEnumValue`.
- Let explicit attributes take precedence over conventions.
- Keep attributes optional and preserve external configuration as the preferred approach for domain-model isolation.

**Priority:** Low-Medium.

## Improvements That Should Remain Outside the Core

- Provider-native connection types in `IDbSession`.
- PostgreSQL bulk-copy helpers in the generic package.
- Global Dapper naming configuration that affects the entire process.
- Regular expressions as the only mechanism for classifying write operations.
- Complete SQL statements or parameter values in telemetry by default.
- Unvalidated session-setting commands assembled from consumer input.

## Suggested Roadmap

### Phase 1: Safer Ergonomics

- Add `DbSessionCallOptions`.
- Add transaction callback helpers.
- Add safe callback overloads for `QueryMultiple`.
- Document session scope and thread-safety requirements.

### Phase 2: Lifecycle and Performance

- Add connection scopes without transactions.
- Add asynchronous result streaming.
- Add controlled underlying-connection callbacks.
- Expand multi-provider lifecycle and disposal tests.

### Phase 3: Observability and Guardrails

- Add standard activities, metrics, and slow-query thresholds.
- Add optional write guards.
- Document telemetry privacy defaults.

### Phase 4: Provider Extensions

- Add validated session-setting formatters.
- Add advanced PostgreSQL extensions.
- Evaluate equivalent capabilities for other supported providers.
- Add configurable Dapper type handlers and modern .NET type coverage.

## Proposed Acceptance Criteria

- New behavior remains opt-in or backward compatible.
- Generic capabilities include tests for at least PostgreSQL and SQLite.
- Provider-specific capabilities use dedicated extensions and integration tests.
- Streaming tests verify cancellation and disposal after partial enumeration.
- Transaction helpers verify commit, rollback, nested behavior, and callback failures.
- Connection-scope tests verify nested scopes and connection ownership.
- Telemetry avoids parameter values and complete statements by default.
- Public documentation clearly distinguishes provider-neutral and provider-specific APIs.

## Conclusion

The proposed improvements apply to `Flowsy.Db.Unity` and align with its purpose as a reusable multi-provider database access library. The strongest near-term opportunities are per-call options, transaction helpers, safe multiple-result consumption, connection scopes, streaming, and standard observability.

The existing convention, provider, parameter, connection-hub, and session abstractions provide a solid foundation for these additions. Provider-specific features should remain optional extensions so the core package stays predictable and broadly reusable.
