# GitHub Environment Inventory

This directory versions the public configuration contract required by release workflows. Files ending in `*.ref.env` declare names and safe reference values only. Real `*.env` files are ignored by Git and must never be committed.

> [!WARNING]
> Never store tokens, API keys, or other sensitive values here. Configure secrets directly in GitHub and keep them out of logs.

## NuGet Production

Package publication uses the `nuget-production` GitHub Environment. Configure it with required reviewers when the repository plan supports them and restrict deployment tags to:

```text
Flowsy.Db.Unity/v*
Flowsy.Db.Unity.Postgres/v*
```

| Type | Name | Purpose |
|---|---|---|
| Variable | `DOTNET_SDK_VERSION` | SDK used to build, test, and pack. |
| Variable | `NUGET_SOURCE_URL` | NuGet.org v3 service index. |
| Secret | `NUGET_API_KEY` | API key allowed to publish the Flowsy packages. |

Copy each `*.ref.env` file under [`nuget/production`](nuget/production/) to its corresponding `*.env` file for local inventory work, replace placeholders, and keep the resulting files outside staging.

The [`publish-nuget.yml`](../workflows/publish-nuget.yml) workflow validates an annotated package-qualified tag, verifies its project version and default-branch ancestry, builds and tests the solution on a standard GitHub-hosted runner, packs the selected project, and publishes the exact package to NuGet.org.
