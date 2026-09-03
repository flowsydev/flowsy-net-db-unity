# Flowsy.Db.Unity Usage Guide

This guide explains how to integrate and use `Flowsy.Db.Unity` in .NET applications.

## Topics

1. [Installation And Configuration](01-installation-and-configuration.md)
2. [Connections, Hubs, And Sessions](02-connections-hubs-and-sessions.md)
3. [Queries And Commands](03-queries-and-commands.md)
4. [Stored Procedures And Functions](04-routines.md)
5. [Naming And Parameter Conventions](05-conventions.md)
6. [Result And Type Mapping](06-result-and-type-mapping.md)
7. [Scripts And Migrations](07-scripts-and-migrations.md)
8. [Transactions, Observability, And Guardrails](08-transactions-and-logging.md)
9. [Extensibility](09-extensibility.md)

## Quick Start

```csharp
using Flowsy.Db.Unity;
using Flowsy.Db.Unity.Postgres;

services.AddDatabases(options =>
{
    options
        .UsePostgres("Store", connectionString)
        .AsDefault()
        .WithConventions()
        .ForRoutines(DbRoutineType.StoredFunction)
        .ForParameters(prefix: "p_", useNamedParameters: true)
        .WithDefault(DbCaseStyle.LowerSnakeCase);
});
```
