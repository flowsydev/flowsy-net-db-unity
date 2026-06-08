# Installation and Configuration

Install the library and the ADO.NET provider required by your database:

```bash
dotnet add package Flowsy.Db.Unity
dotnet add package Npgsql
```

Register one or more connections through `AddDatabases`:

```csharp
services.AddDatabases(options =>
{
    options
        .UseConnection("Store", configuration.GetConnectionString("Store")!)
        .AsDefault()
        .WithProvider(DbProviderFamily.Postgres, "Npgsql", NpgsqlFactory.Instance)
        .WithLogLevel(LogLevel.Information)
        .WithConventions()
        .WithDefault(DbCaseStyle.LowerSnakeCase);
});
```

`DbProviderFamily` supports PostgreSQL, SQL Server, MySQL, Oracle, SQLite, and generic ADO.NET providers. `AddDatabases` registers `IDbConnectionFactory` as a singleton and the session factory and connection hub as scoped services.
