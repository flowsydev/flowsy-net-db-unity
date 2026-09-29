# Changelog

All notable changes to `Flowsy.Db.Unity.Postgres` are documented in this file.

## [Unreleased]

## [1.0.0] - 2026-09-28

### Added

- Add opt-in PostgreSQL integration for `Flowsy.Db.Unity` 6.0.0 or later on .NET 8 and .NET 10.
- Add `UsePostgres` and `WithPostgres` configuration extensions backed by Npgsql.
- Create and reuse an `NpgsqlDataSource` per connection configuration, using reference identity to avoid hashing cyclic convention graphs.
- Map PostgreSQL enums from provider-neutral conventions, including explicit member values and custom name translators.
- Add composite type mappings through `MapComposite<T>`.
- Dispose cached data sources with their connection provider, supporting synchronous and asynchronous disposal.
- Include package-specific usage documentation and IntelliSense XML documentation.
