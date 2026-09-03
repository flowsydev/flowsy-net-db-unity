# Changelog

All notable changes to `Flowsy.Db.Unity.Postgres` are documented in this file.

## [Unreleased]

### Fixed

- Key reusable data sources by configuration identity without hashing the cyclic convention graph.

## [1.0.0] - 2026-09-02

### Added

- Reusable `NpgsqlDataSource` instances per connection configuration.
- PostgreSQL enum and composite mapping support.
