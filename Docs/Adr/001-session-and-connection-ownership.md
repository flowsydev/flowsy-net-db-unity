# ADR 001: Session And Connection Ownership

**Status:** Accepted.

## Context

`IDbSession` must retain a consistent connection for transactions, streaming, and native operations.

## Decision

Each session reuses one `IDbConnection` throughout its lifetime. `DbConnectionUsage` determines whether the session disposes or only closes it. Callbacks never receive ownership of the connection or transaction.

## Consequences

Separate connection scopes without transactions are unnecessary because the session already defines that lifetime. Streams and readers must finish before the session is disposed.
