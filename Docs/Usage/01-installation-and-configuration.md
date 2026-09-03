# Installation And Configuration

Install the core library and the ADO.NET provider required by your database:

```bash
dotnet add package Flowsy.Db.Unity
dotnet add package Npgsql
```

For PostgreSQL, install the opt-in integration package instead of adding Npgsql directly. It includes the compatible core package and reusable data-source configuration:

```bash
dotnet add package Flowsy.Db.Unity.Postgres
```

Register PostgreSQL through its provider extension:

```csharp
services.AddDatabases(options =>
{
    options
        .UsePostgres("Store", configuration.GetConnectionString("Store")!)
        .AsDefault()
        .WithLogLevel(LogLevel.Information)
        .WithConventions()
        .WithDefault(DbCaseStyle.LowerSnakeCase);
});
```

For other engines, use `UseConnection(...).WithProvider(...)` with the required ADO.NET factory. `DbProviderFamily` supports PostgreSQL, SQL Server, MySQL, Oracle, SQLite, and generic ADO.NET providers.

`AddDatabases` registers `IDbConnectionFactory` as a singleton and the session factory and connection hub as scoped services. Keep `IDbConnectionHub` and each `IDbSession` within their scope.
