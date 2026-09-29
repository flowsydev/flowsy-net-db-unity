# Changelog

All notable changes to `Flowsy.Db.Unity.Postgres` are documented in this file.

## [Unreleased]

## [2.0.0] - 2026-09-28

### Changed

- **BREAKING**: Require `Flowsy.Db.Unity` 6.0.0 or later. Consumers must account for its expanded `IDbSession` contract and rebuild assemblies that use its changed constructor signatures.

### Fixed

- Key reusable data sources by configuration identity without hashing the cyclic convention graph.
- Correct the core package changelog link in the package README.

## [1.0.0] - 2026-09-02

### Added

- Reusable `NpgsqlDataSource` instances per connection configuration.
- PostgreSQL enum and composite mapping support.
