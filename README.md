# Flowsy DB Unity

.NET libraries for SQL database access through provider-neutral contracts, configurable conventions, and opt-in provider extensions.

## Packages

| Package | Purpose | Current Version | Documentation |
|---|---|---:|---|
| `Flowsy.Db.Unity` | Provider-neutral connections, sessions, queries, routines, transactions, migrations, conventions, and observability. | `5.0.0` | [README](Flowsy.Db.Unity/README.md) · [Changelog](Flowsy.Db.Unity/CHANGELOG.md) |
| `Flowsy.Db.Unity.Postgres` | PostgreSQL configuration through reusable `NpgsqlDataSource` instances without adding provider APIs to `IDbSession`. | `1.0.0` | [README](Flowsy.Db.Unity.Postgres/README.md) · [Changelog](Flowsy.Db.Unity.Postgres/CHANGELOG.md) |

Each package has its own version, changelog, and release cycle. Tags use `<PackageId>/v<SemVer>`, such as `Flowsy.Db.Unity/v5.0.0`.

## Repository Structure

| Path | Contents |
|---|---|
| [`Flowsy.Db.Unity/`](Flowsy.Db.Unity/) | Core package source and documentation. |
| [`Flowsy.Db.Unity.Postgres/`](Flowsy.Db.Unity.Postgres/) | Opt-in PostgreSQL provider extension. |
| [`Flowsy.Db.Unity.Test/`](Flowsy.Db.Unity.Test/) | Shared unit and integration tests. |
| [`Docs/Usage/`](Docs/Usage/) | Detailed usage guides. |
| [`Docs/Adr/`](Docs/Adr/) | Current architectural decisions. |

## Development

```shell
dotnet restore Flowsy.Db.Unity.sln
dotnet build Flowsy.Db.Unity.sln
dotnet test Flowsy.Db.Unity.sln
```

Integration tests require Docker because they use Testcontainers.

To create both packages locally:

```shell
dotnet pack Flowsy.Db.Unity/Flowsy.Db.Unity.csproj --configuration Release --include-symbols
dotnet pack Flowsy.Db.Unity.Postgres/Flowsy.Db.Unity.Postgres.csproj --configuration Release --include-symbols
```

## Releases

An annotated package tag starts the release workflow. The workflow validates the tag against the selected project version, builds and tests the solution, packs only that project, and publishes it to NuGet.org through the protected `nuget-production` environment.

See [ADR 007: Package Versioning And Tags](Docs/Adr/007-package-versioning-and-tags.md) for the complete versioning strategy.
