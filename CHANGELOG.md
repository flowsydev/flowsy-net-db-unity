# Changelog

All changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/) and this project adheres to [Semantic Versioning](https://semver.org/).

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
