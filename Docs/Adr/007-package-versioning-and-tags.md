# ADR 007: Package Versioning And Tags

**Status:** Accepted.

## Context

The repository originally published only `Flowsy.Db.Unity`, so historical tags contain only a version. Adding `Flowsy.Db.Unity.Postgres` introduces two NuGet packages with potentially independent release cycles and makes repository-wide version tags ambiguous.

## Decision

Each package has its own `CHANGELOG.md`, `Version` property, and independent Semantic Versioning sequence.

New annotated tags use:

```text
<PackageId>/v<SemVer>
```

Examples:

```text
Flowsy.Db.Unity/v5.1.0
Flowsy.Db.Unity.Postgres/v1.0.0
```

Historical tags without a package ID are migrated once to `Flowsy.Db.Unity/v<SemVer>`. Each new tag preserves the original commit. After the new remote references are verified, old local and remote tags are removed to avoid two names for the same release.

Each tag points to the commit containing its published version and changelog entry. If one commit releases both packages, it receives two independent tags. When the extension depends on a new core version, the core package is published first.

## Consequences

- Releasing one package does not require incrementing the other when its artifact and public contract are unchanged.
- Release workflows select the project from the package ID in the tag.
- GitHub Releases use the package ID and version as their title.
- Published tags are immutable; later corrections require a new package version.
- Links and automation must use qualified tag names.

## Historical Migration

Versions `1.0.0` through `5.0.0` belong to `Flowsy.Db.Unity` and become `Flowsy.Db.Unity/v<SemVer>`. Migration changes neither their target commit nor the already published packages.
