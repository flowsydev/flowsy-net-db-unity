---
name: flw-release-nuget
description: Explicit command for preparing, publishing, or retrying NuGet releases of Flowsy.Db.Unity and Flowsy.Db.Unity.Postgres. Use only when the user explicitly invokes `$flw-release-nuget`; with no arguments, show help and perform no operation. Never trigger from natural language.
---

# NuGet Release

Manage independent releases for this repository without moving existing tags or publishing directly outside the GitHub workflow.

## Invocation

```text
$flw-release-nuget prepare <package>
$flw-release-nuget prepare <package> <version>
$flw-release-nuget publish <package> <version>
$flw-release-nuget retry <package> <version>
```

The equivalent `/flw-release-nuget` forms are also valid. Do not trigger from natural language or inferred intent. If arguments are missing, invalid, or excessive, show help and stop without inspecting or modifying the repository.

Supported packages:

- `Flowsy.Db.Unity`
- `Flowsy.Db.Unity.Postgres`

`<version>` must be an exact stable SemVer without a `v` prefix.

## Package Map

| Package | Project | Changelog | Tag |
|---|---|---|---|
| `Flowsy.Db.Unity` | `Flowsy.Db.Unity/Flowsy.Db.Unity.csproj` | `Flowsy.Db.Unity/CHANGELOG.md` | `Flowsy.Db.Unity/v<version>` |
| `Flowsy.Db.Unity.Postgres` | `Flowsy.Db.Unity.Postgres/Flowsy.Db.Unity.Postgres.csproj` | `Flowsy.Db.Unity.Postgres/CHANGELOG.md` | `Flowsy.Db.Unity.Postgres/v<version>` |

## Authorization Boundaries

- `prepare <package>` authorizes analysis and a version proposal only.
- `prepare <package> <version>` authorizes updating only the selected project and changelog, validating them, creating one commit, and pushing `dev`.
- `publish` authorizes creating and pushing exactly one annotated tag, monitoring its workflow, and creating or reusing its GitHub Release after successful publication.
- `retry` authorizes one manual dispatch for exactly one existing tag and monitoring that run.
- No operation authorizes merging pull requests, moving tags, force pushing, changing GitHub environments, publishing directly with `dotnet nuget push`, or altering the other package.

## Common Preconditions

1. Read `AGENTS.md`, the selected project and changelog, `.github/workflows/publish-nuget.yml`, and the versioning ADR.
2. Confirm `origin` is `flowsydev/flowsy-net-db-unity` and `gh auth status` permits the required operations.
3. Refresh remote references with `git fetch origin --prune`; never pull, rebase, reset, or force push.
4. Require a completely clean worktree on local `dev`, exactly synchronized with `origin/dev`.
5. Resolve tags and branches explicitly and dereference annotated tag objects.
6. Never read or expose secrets, tokens, API keys, or connection strings.
7. Stop on state mismatches with concrete evidence and a safe next action.

## Prepare Analysis

1. Find the latest package-qualified SemVer tag and compare its dereferenced commit with `HEAD`.
2. Review the entire change range and attribute each change to the selected package or shared infrastructure.
3. Propose the minimum Semantic Versioning increment:
   - `major`: public compatibility or documented behavior break.
   - `minor`: compatible new API or behavior.
   - `patch`: compatible package fix.
   - no release: documentation, tests, skills, CI, or maintenance without artifact impact.
4. If Postgres requires a new core package, require preparing the core first.
5. Present the current and proposed versions, rationale, files that would change, validation, and the `dev` destination.
6. Stop without changes and show the explicit command required to apply the proposal.

## Prepare With Explicit Version

1. Recalculate the minimum increment and require the supplied version to match exactly.
2. Repeat every precondition and ensure `HEAD` did not change during analysis.
3. Update only the selected project's `Version` and add the top changelog entry using the local `America/Mexico_City` date.
4. Write the changelog in English using applicable Keep a Changelog sections and only selected-package changes.
5. Run at minimum:

```shell
dotnet restore
dotnet build --configuration Release --no-restore
dotnet test --configuration Release --no-build
dotnet pack <project> --configuration Release --no-build --include-symbols
git diff --check
```

6. Validate the generated package name and contents without tracking `bin` or `obj`.
7. Create one detailed English Conventional Commit with scope `release`.
8. Push only `dev` with `git push origin dev:dev`, verify the remote commit, and report `$flw-release-pr dev` as the next explicit command.

## Publish

1. Require the requested version to match the project and top changelog entry.
2. Require `HEAD` to be the prepared `dev` commit and an ancestor of `origin/main`.
3. Require the exact tag to be absent locally and remotely. If it exists, recommend `retry`; never recreate it.
4. Validate the workflow contract for package-qualified tags, the `nuget-production` environment, exact project resolution, NuGet.org, and required configuration.
5. If Postgres depends on a new core version, require the exact core-tag workflow to have succeeded.
6. Create an annotated tag with message `Release <PackageId> v<version>` on the validated commit.
7. Push only that ref with `git push origin refs/tags/<tag>:refs/tags/<tag>`.
8. Identify the run caused by that push using workflow, event, tag, and SHA, then monitor Resolve and Publish.
9. After success, create or reuse a GitHub Release with the same tag, title `<PackageId> v<version>`, and notes derived only from the matching changelog entry.
10. Report the tag, SHA, workflow URL and jobs, package, and release URL. Never retry automatically after a failure.

## Retry

1. Require the exact local and remote tag to exist, be annotated, and dereference to the same commit.
2. Require that commit to be an ancestor of `origin/main`, with matching project and changelog versions.
3. If an equivalent successful workflow already exists, return its URL and stop.
4. Dispatch `publish-nuget.yml` once from the default branch with `release_tag=<tag>`.
5. Identify and monitor the new run by ID, event, exact input, and dispatch time.
6. On success, create or reuse the GitHub Release under the same rules as `publish`.
7. Never delete, recreate, move, or force a tag.

## Failure Handling

Treat a partially created remote ref as the source of truth. If only a local tag exists after a failed push, report that state and do not delete it without a separate explicit instruction. Preserve logs and avoid further mutations after validation or publication failures.

## Final Report

Distinguish initial and final state, exact package, version, commit and tag, modified files and actual checks, created refs or remote resources, actions omitted for safety, and the next explicit command when applicable.
