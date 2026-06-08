# Flowsy.Db.Unity Usage Guide

This guide explains how to integrate and use `Flowsy.Db.Unity` in .NET applications.

## Topics

1. [Installation and configuration](01-installation-and-configuration.md)
2. [Connections, hubs, and sessions](02-connections-hubs-and-sessions.md)
3. [Queries and commands](03-queries-and-commands.md)
4. [Stored procedures and functions](04-routines.md)
5. [Naming and parameter conventions](05-conventions.md)
6. [Result and type mapping](06-result-and-type-mapping.md)
7. [Scripts and migrations](07-scripts-and-migrations.md)
8. [Transactions and logging](08-transactions-and-logging.md)
9. [Extensibility](09-extensibility.md)

## Quick Start

```csharp
using Flowsy.Db.Unity;
using Npgsql;

services.AddDatabases(options =>
{
    options
        .UseConnection("Store", connectionString)
        .AsDefault()
        .WithProvider(DbProviderFamily.Postgres, "Npgsql", NpgsqlFactory.Instance)
        .WithConventions()
        .ForRoutines(DbRoutineType.StoredFunction)
        .ForParameters(prefix: "p_", useNamedParameters: true)
        .WithDefault(DbCaseStyle.LowerSnakeCase);
});
```
