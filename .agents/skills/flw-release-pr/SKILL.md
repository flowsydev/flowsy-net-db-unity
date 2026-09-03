---
name: flw-release-pr
description: Explicit command for creating or reusing the pull request that promotes dev to main in this repository. Use only when the user explicitly invokes `$flw-release-pr`; with no arguments, show help and perform no operation. Never trigger from natural language.
---

# Release Pull Request

Create or reuse the only supported promotion pull request from `dev` to `main`, validating both NuGet packages in this repository.

## Invocation

Valid command forms:

```text
$flw-release-pr dev
/flw-release-pr dev
```

Do not trigger from context, inferred intent, a mention of this skill, or a natural-language release request. Skills do not invoke one another.

The only valid argument is `dev`. If it is missing, different, or accompanied by extra arguments, show the valid forms and stop without inspecting or modifying the repository.

## Authorization Boundaries

A valid invocation authorizes read-only repository and GitHub inspection, local non-destructive validation, and creating exactly one pull request with `head=dev` and `base=main`, or reusing the matching open pull request.

It does not authorize editing files, creating branches or commits, merging, approving or closing pull requests, creating or moving tags, dispatching publication workflows, publishing packages, force pushing, rebasing, resetting, or switching branches.

## Preconditions

1. Read `AGENTS.md`, both package projects and changelogs, the NuGet workflow, and the versioning ADR.
2. Confirm `origin` is `flowsydev/flowsy-net-db-unity` and `gh auth status` permits pull-request queries and creation.
3. Require a completely clean worktree, including untracked files.
4. Require the current branch to be `dev` at exactly `origin/dev`.
5. Refresh remote references only with `git fetch origin --prune`; never use `pull`.
6. Resolve `origin/dev` and `origin/main`. If `dev` is already an ancestor of `main`, report that no promotion is pending and stop.
7. Inspect the complete `origin/main..origin/dev` range, including commits, APIs, projects, changelogs, documentation, CI, and skills.
8. Run at minimum:

```shell
dotnet restore
dotnet build --configuration Release --no-restore
dotnet test --configuration Release --no-build
git diff --check origin/main...origin/dev
```

Do not create a pull request if any check fails.

## Package Release Validation

For each package:

1. Read the current project version and top changelog entry.
2. Resolve the newest package-qualified SemVer tag and dereference annotated tags.
3. Determine whether the range prepares a new package version, contains support-only changes, or includes a tag whose commit is not yet in `main`.
4. When a version exceeds the latest tag, require a matching changelog entry consistent with the changes.
5. If Postgres depends on a new core version, state that the core package must be published first after integration.
6. Do not block promotion solely because a tag already exists before integration. Report the anomaly and prescribe `retry` after a person merges the pull request. Never suggest moving the tag.

## Idempotency And Collisions

Query open and closed pull requests from `dev` to `main`.

- Reuse a matching open pull request, return its URL, and do not create another.
- Stop on a closed, unmerged pull request for the same `dev` state.
- A new pull request is allowed after an earlier one was merged and `dev` diverged again.
- Never reuse or close a pull request with reversed or different branches.

## Pull Request Creation

Create exactly `base=main`, `head=dev` in the same repository. Use the title:

```text
chore(release): merge dev into main
```

Write the body in English with:

```markdown
## Objective

## Packages And Versions

## Included Changes

## Validation

## Publication Risks And Coordination
```

Distinguish prepared versions, unchanged artifacts, and existing tags. Record only checks that actually ran. State the package publication order and commands that may be run only after a human merge: `$flw-release-nuget publish <package> <version>` for a new tag or `$flw-release-nuget retry <package> <version>` for an existing tag.

After creation, query the pull request again and verify its URL, head, base, state, and commits.

## Final Report

Report the pull request URL and number, examined branch commits, detected package versions, actual validation results, preexisting tags or publication dependencies, and confirm that nothing was merged, tagged, or published.
