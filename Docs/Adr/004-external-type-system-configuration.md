# ADR 004: External Type-System Configuration

**Status:** Accepted.

## Context

Persistence attributes on models would expose physical details and create competing sources of truth.

## Decision

Exceptional parameter names and enum values are declared in per-connection conventions. Dapper type handlers are explicitly registered as global. A handler takes precedence over `DbType` inference and conventional transformations.

## Consequences

Models remain free of library attributes. PostgreSQL enums use mappings from `ForEnums`; composites, which have no provider-neutral representation, are configured by the PostgreSQL package.
