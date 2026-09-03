# Changelog

All changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/) and this project adheres to [Semantic Versioning](https://semver.org/).

---

## [Unreleased]

### Added

- Add provider-neutral connection extensions and the opt-in `Flowsy.Db.Unity.Postgres` integration.
- Add per-call command options, transaction helpers, progressive streaming, safe multiple-result callbacks, and controlled native connection access.
- Add external parameter and enum value mappings, configurable Dapper type handlers, and `DateOnly` and `TimeOnly` handlers.
- Add tracing, metrics, slow-operation warnings, write transaction guards, and scoped allowlisted session settings.

### Fixed

- Match constructor parameters without accepting inconsistent lengths or reading past the final parameter.
- Preserve the requested isolation level when beginning an asynchronous transaction.
- Append PostgreSQL array suffixes only when a custom database type is present.
- Clean already applied session settings when applying a later setting fails.
- Count multiple-result reader consumption as part of its command instead of as a second database command.
- Override vulnerable transitive SQLite and SSH test dependencies with corrected releases.

### Changed

- Generate and package IntelliSense XML documentation, and fail builds when public APIs are undocumented or contain inconsistent parameter tags.
- Restructure repository and package documentation as focused, provider-aware guides with package-specific READMEs.

---

## [5.0.0] - 2026-06-08

### Added

- Add support for `net10.0`.
- Add an English usage guide under `Docs/Usage`.
- Add repository-level agent instructions in `AGENTS.md`.

### Changed

- **BREAKING**: Change supported target frameworks from `net6.0` and `net8.0` to `net8.0` and `net10.0`.
- Use conditional dependency versions aligned with the `net8.0` and `net10.0` targets.
- Run the integration test project against both `net8.0` and `net10.0`.
- Update shared dependencies to their latest stable versions.
- Migrate the test project to xUnit v3.
- Replace the external xUnit ordering extension with internal orderers based on public xUnit v3 APIs.
- Run target frameworks and dependent integration-test collections sequentially for deterministic database scenarios.
- Update Testcontainers usage for the latest stable API.
- Use `DbType.DateTime2` for `DateTime` parameters and normalize their kind before sending them to providers.

### Fixed

- Ensure dependent PostgreSQL integration scenarios execute in their required order.
- Read the package version from `<Version>` in `publish.sh`.

### Removed

- Remove support for `net6.0`.
- Remove obsolete repository and test-project Copilot instruction files.

---

## [4.0.4] - 2025-11-15

### Fixed

- Fix parameter construction to allow passing array-type values

---

## [4.0.3] - 2025-09-25

### Fixed

- Fix log errors in DbSession.QuerySingleAsync and DbSession.QuerySingleOrDefaultAsync to standardize database operation details

### Changed

- Add missing XML Documetation to DbSession class methods

---

## [4.0.2] - 2025-09-24

### Fixed

- Add casting for dynamic parameters in DbSession to prevent runtime errors

---

## [4.0.1] - 2025-09-24

### Changed

- Added log details for database operations in DbSession class

---

## [4.0.0] - 2025-09-14

### Changed

- **BREAKING**: Replaced DbAgent/DbUnitOfWork with simplified DbSession architecture for improved performance and maintainability
- Streamlined database session management with new DbSession pattern

### Added

- New DbSession class with comprehensive database operation support
- DbSessionFactory for creating and managing database sessions
- Enhanced connection management through DbConnectionHub and DbConnectionFactory
- Improved database parameter handling with DbParameterBuilder
- Support for multiple database providers with DbProviderDescriptor
- Database migration capabilities through DbMigrationConfiguration
- Comprehensive support for database conventions with DbConventionSet
- New extension methods for enhanced query operations

### Improved

- Better resource management and disposal patterns
- Enhanced error handling and logging capabilities
- Optimized connection pooling and lifecycle management
- Improved type mapping and parameter binding

---

## [3.0.0] - 2025-06-11

### Changed

- Database connection management improvement

---

## [2.0.5] - 2025-06-09

### Added

- Logs when disposing connections

---

## [2.0.4] - 2025-06-09

### Changed

- Modified creation of the default DbAgent instance to use IDbConnectionFactory instead of IDbConnectionScope

---

## [2.0.3] - 2025-06-09

### Fixed

- GetFirstOrDefault and GetSingleOrDefault extension methods to avoid error when no results are found

---

## [2.0.2] - 2025-06-09

### Added

- Support for Nullable types when building parameter descriptors

---

## [2.0.1] - 2025-06-06

### Added

- Constructor for DbEnumMapping to allow creation without specifying an instance of DbConventionSet

---

## [2.0.0] - 2025-06-01

### Changed

- Refactored DbAgent and DbUnitOfWork to improve management and disposal of the underlying IDbConnection object

---

## [1.2.1] - 2025-06-01

### Fixed

- DbAgent.DisposeAsync method now invokes GC.SuppressFinalize

---

## [1.2.0] - 2025-05-24

### Added

- Method to obtain the required DbConnectionOptions instance

---

## [1.1.0] - 2025-05-24

### Added

- Property to expose the collection of enum mappings

---

## [1.0.0] - 2025-05-17

### Added

- Initial stable release
- Reference to README.md file in csproj file
- Complete documentation
- Strict mode for type mapping in database queries
- IDbUnitOfWorkParticipant interface and DbUnitOfWorkParticipant class to allow services to be involved in units of work
- Comprehensive XML documentation for all classes and methods
- Database queries based on conventions
- Foundation interfaces and classes
