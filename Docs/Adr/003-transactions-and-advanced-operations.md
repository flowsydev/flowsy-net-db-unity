# ADR 003: Transactions And Advanced Operations

**Status:** Accepted.

## Context

Manual transaction handling makes it easy to omit rollback, while some advanced operations require access to the native driver.

## Decision

`InTransactionAsync` manages a new transaction. `InExistingOrNewTransactionAsync` respects an outer transaction and manages only the transaction it creates. `WithConnectionAsync` temporarily provides the underlying connection and transaction, with typed and result-returning variants.

## Consequences

Manual APIs remain available. Callbacks must not close, dispose, or complete session-owned resources. Typed access is an explicit and localized escape hatch for provider-specific operations.
