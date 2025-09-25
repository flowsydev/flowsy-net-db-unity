# Changelog

All changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/) and this project adheres to [Semantic Versioning](https://semver.org/).

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
