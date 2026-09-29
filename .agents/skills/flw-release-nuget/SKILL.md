---
name: flw-release-nuget
description: Explicit one-command workflow for analyzing, approving, preparing, committing, pushing, tagging, and launching a NuGet release of Flowsy.Db.Unity or Flowsy.Db.Unity.Postgres. Use only when the user invokes `$flw-release-nuget` with one supported package; never trigger from natural language.
---

# NuGet Release

Release exactly one package through an interactive approval checkpoint. The invocation starts the analysis; it does not authorize mutations until the user approves the exact version and changelog proposal.

## Invocation

```text
$flw-release-nuget Flowsy.Db.Unity
$flw-release-nuget Flowsy.Db.Unity.Postgres
```

The equivalent `/flw-release-nuget <package>` form is valid. If the package is missing, unsupported, or accompanied by extra arguments, show these forms and stop without inspecting the repository.

| Package | Project | Changelog | Tag |
|---|---|---|---|
| `Flowsy.Db.Unity` | `Flowsy.Db.Unity/Flowsy.Db.Unity.csproj` | `Flowsy.Db.Unity/CHANGELOG.md` | `Flowsy.Db.Unity/v<version>` |
| `Flowsy.Db.Unity.Postgres` | `Flowsy.Db.Unity.Postgres/Flowsy.Db.Unity.Postgres.csproj` | `Flowsy.Db.Unity.Postgres/CHANGELOG.md` | `Flowsy.Db.Unity.Postgres/v<version>` |

## Authorization

A valid invocation authorizes repository and GitHub inspection only. Pause after presenting the proposal and ask the user to approve or reject it.

Approval authorizes only the proposed package, version, changelog text, release commit, push to `dev`, and annotated tag. It does not authorize altering the other package, merging or creating a pull request, moving an existing tag, force pushing, changing GitHub environments, or publishing directly with `dotnet nuget push`.

Treat an ambiguous response or any requested adjustment as no approval. Revise the proposal and ask again.

## Preconditions

1. Read `AGENTS.md`, the selected project and changelog, `.github/workflows/publish-nuget.yml`, and the versioning ADR.
2. Confirm `origin` is `flowsydev/flowsy-net-db-unity` and `gh auth status` permits branch, tag, and workflow inspection.
3. Fetch with `git fetch origin --prune`; never pull, rebase, reset, switch branches, or force push.
4. Require a completely clean worktree on local `dev`, including untracked files.
5. Require `origin/dev` to be an ancestor of `HEAD`. This permits committed local release changes while preventing a non-fast-forward push.
6. Resolve package-qualified tags explicitly and dereference annotated tag objects.
7. Never read or expose secrets, API keys, tokens, or connection strings.

Stop on any mismatch and provide the smallest safe corrective action.

## Analyze and Propose

1. Find the latest stable package-qualified tag and compare its dereferenced commit with `HEAD`.
2. Review the complete change range and attribute changes to the selected package or shared runtime infrastructure.
3. If there is no artifact-impacting change, report that no release is needed and stop.
4. Propose the minimum Semantic Versioning increment:
   - `major` for a public compatibility or documented behavior break;
   - `minor` for compatible API or behavior additions;
   - `patch` for compatible fixes;
   - no release for documentation, tests, skills, CI, or maintenance without artifact impact.
5. Draft the exact English Keep a Changelog entry using only selected-package changes.
6. If Postgres needs an unreleased core version, stop and require releasing `Flowsy.Db.Unity` first.
7. Verify the proposed tag is absent locally and remotely.
8. Present the package, current version and tag, proposed version and tag, rationale, exact changelog entry, files to edit, validations, commit destination, and publication effect.
9. Ask for explicit authorization to apply that exact proposal, then pause.

## Apply the Approved Release

After approval:

1. Fetch again and require the worktree, `HEAD`, `origin/dev`, latest package tag, and proposal inputs to match the analyzed state.
2. Update only the selected project `Version` and selected changelog. Add the approved entry with the current `America/Mexico_City` date and preserve an empty `Unreleased` section.
3. Run:

```shell
dotnet restore
dotnet build --configuration Release --no-restore
dotnet test --configuration Release --no-build
dotnet pack <project> --configuration Release --no-build --include-symbols
git diff --check
```

4. Validate the exact package filename and inspect its README, assemblies, symbols, and IntelliSense XML documentation. Do not track `bin` or `obj`.
5. Review and stage only the selected project and changelog.
6. Create one detailed English Conventional Commit with scope `release`. Include the version rationale, changelog summary, validation, and publication implications.
7. Push only `dev` with `git push origin dev:dev` and verify `origin/dev` equals the release commit.
8. Create the annotated tag `<PackageId>/v<version>` on that commit with message `Release <PackageId> v<version>`.
9. Push only that tag ref. Never use `--tags` or recreate an existing tag.
10. Identify the workflow caused by the tag push using workflow, event, tag, and SHA. Monitor its Resolve and Publish jobs and report their URLs and actual status; never retry automatically.
11. Report `$flw-release-pr dev` as the next explicit command. Do not invoke the PR skill.

## Failure Handling

- If validation or the branch push fails, do not create the tag.
- If a local tag exists after its push fails, preserve it and report the exact local and remote state.
- Treat any created remote ref as the source of truth. Never delete, move, or overwrite it automatically.
- If the publication workflow fails, preserve its URL and logs and stop without dispatching another run.

## Final Report

Report the initial and final state, approved proposal, package, version, release commit, tag, branch and tag push results, validations, package contents, workflow jobs, omitted actions, and the explicit PR command.
