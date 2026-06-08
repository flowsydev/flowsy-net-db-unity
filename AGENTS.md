# AGENTS.md

## Repo

.NET library for SQL database access with configurable conventions for names, parameters, routines, enums, date-time values, migrations, and logging. The main library lives in `Flowsy.Db.Unity/`; xUnit tests, fixtures, and database scripts live in `Flowsy.Db.Unity.Test/`.

Primary stack: C# with `net8.0` and `net10.0`, Dapper, Evolve, Microsoft Extensions, xUnit v3, Shouldly, and Testcontainers. Both the library and test projects target .NET 8 and .NET 10.

## Specific Rules

- Follow the Flowsy DevGuide as the cross-cutting reference, especially `Repository Documentation`, `Writing Guidelines`, and `Repository Agent Instructions`.
- Write durable documentation, XML Docs, code comments, changelog entries, and user-facing examples in clear English by default. Use Spanish only for concepts that are intrinsically Mexican or legal/business-specific, such as CURP or RFC.
- Keep Flowsy naming consistent: friendly name `Flowsy`, GitHub organization `flowsydev`, command and artifact prefixes `flw` or `flw-`, and .NET packages `Flowsy.*`.
- Use examples and terminology suitable for a general open source community. Avoid institutional or private-domain terminology.
- Write titles, headings, and navigation labels in Title Case.
- Use `PascalCase` connection keys and C# dynamic parameter properties.
- `IDbConnectionFactory` creates connections from `DbConnectionConfiguration`; `IDbConnectionHub` manages connections and must work as a scoped service; `IDbSession` wraps Dapper operations and routines.
- Keep `IDbSession` and `DbSession` organized as `partial` types with separate files for `Execute`, `ExecuteRoutine`, `ExecuteScript`, `Migrations`, `Query`, `QueryFirst`, `QueryMultiple`, and `QuerySingle` operation groups.
- Preserve SQL and routine overloads for session wrappers, including `First`, `FirstOrDefault`, `Single`, and `SingleOrDefault` variants.
- Log session operation start, success, and failure with `ILogger`. Messages must start with `[ SESSION:{SessionId} > OP:{OperationId} ]` and include useful command, routine, result, or error details.
- XML Docs for `First`, `FirstOrDefault`, `Single`, and `SingleOrDefault` methods must explain behavior when no results or multiple results are returned.
- Use xUnit v3, Shouldly, `ITestOutputHelper`, the AAA pattern, and the internal orderers under `Flowsy.Db.Unity.Test/Infrastructure/Testing/Ordering`.
- Propagate `TestContext.Current.CancellationToken` through asynchronous integration test operations.
- Test migration scripts live under `Flowsy.Db.Unity.Test/Mock/Infrastructure/Database/Scripts/<ConnectionKey>/Migrations/Versioned|Repeatable`.
- Use `lower_snake_case` for database objects except SQL Server objects, which use `PascalCase`. Expected prefixes are views `vw_`/`Vw_`, functions `fn_`/`Fn_`, procedures `sp_`/`Sp_`, and parameters `p_`/`P_`.
- Keep runtime-aligned dependencies in conditional `ItemGroup` elements for `net8.0` and `net10.0`; keep runtime-neutral dependencies in shared groups.

## Commands

Restore and build:

```bash
dotnet restore
dotnet build
```

Run tests:

```bash
dotnet test
```

Integration tests require Docker because they use Testcontainers.

Pack the library:

```bash
dotnet pack Flowsy.Db.Unity/Flowsy.Db.Unity.csproj --configuration Release --include-symbols
```

Publish the package manually, only with explicit approval and a valid API key:

```bash
bash publish.sh
```

## Release Preparation and Publishing

- When explicitly asked to prepare the local repository for publishing, confirm developer approval before continuing. After approval, update `Version` in `Flowsy.Db.Unity/Flowsy.Db.Unity.csproj`, update `CHANGELOG.md`, create a detailed English Conventional Commit, and create the corresponding Git tag.
- When explicitly asked to publish changes to the remote repository or package registry, confirm developer approval before running `git push`, `git push --tags`, `bash publish.sh`, or `nuget push`.
- Do not create commits, tags, or pushes by inference. Ask when the intent is unclear.

## Context on Demand

Read only when required by the task:

- `README.md`: purpose, architecture, configuration examples, and usage overview.
- `Docs/Usage/`: focused usage documentation.
- `Flowsy.Db.Unity/Flowsy.Db.Unity.csproj`: target frameworks, package version, and library dependencies.
- `Flowsy.Db.Unity.Test/Flowsy.Db.Unity.Test.csproj`: test dependencies and integration test configuration.
- `Flowsy.Db.Unity/Resources/Strings.resx` and `Flowsy.Db.Unity/Resources/Strings.es.resx`: localized resources; edit `.resx` files, not generated `.Designer.cs` files.
- Flowsy DevGuide local repository: consult guides by guide and section name instead of relying on fragile internal paths.

## Care

- Do not include secrets, tokens, API keys, real connection strings, sensitive local paths, or production data in versioned files.
- Do not manually edit `bin/`, `obj/`, `.DS_Store`, generated `.Designer.cs` files, or build artifacts unless explicitly requested.
- Do not run `publish.sh`, `nuget push`, commits, tags, or pushes without explicit developer approval.
- Preserve existing changes in the working tree and do not revert unrelated work.
