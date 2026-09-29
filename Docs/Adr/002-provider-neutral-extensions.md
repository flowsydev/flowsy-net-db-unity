# ADR 002: Provider-Neutral Extensions

**Status:** Accepted.

## Context

Some drivers require specialized connection creation without introducing their types into the core package.

## Decision

The core defines `IDbProviderConfiguration` and `IDbConnectionProvider`. Each opt-in package attaches settings to `DbConnectionConfiguration` and creates connections for configurations it recognizes. The generic factory remains the fallback.

## Consequences

`Flowsy.Db.Unity.Postgres` can manage an `NpgsqlDataSource` per connection and dispose it with the container. Other providers can follow the same contract without privileged paths in the core.
