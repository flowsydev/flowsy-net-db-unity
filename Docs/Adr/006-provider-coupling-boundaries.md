# ADR 006: Provider Coupling Boundaries

**Status:** Accepted.

## Context

In lightweight architectures, provider-specific extensions can easily leak into application logic.

## Decision

Provider integrations are limited to application composition and connection creation. No PostgreSQL methods are added to `IDbSession`, and no provider attributes or business-specific builders are introduced. `WithConnectionAsync<TConnection>` is reserved for exceptional infrastructure needs.

## Consequences

Most consumer code depends only on provider-neutral contracts. Changing providers concentrates changes in configuration, persistence, and explicit advanced uses.
