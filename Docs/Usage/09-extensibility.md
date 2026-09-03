# Extensibility

Implement `IDbConnectionFactory` to resolve secrets dynamically, wrap connections, or support custom creation logic:

```csharp
options.UseConnectionFactory<TenantConnectionFactory>();
```

Implement `IDbSessionFactory` to create a custom `DbSession` subtype:

```csharp
options.UseSessionFactory<AuditedSessionFactory>();
```

`WithProvider` accepts any compatible `DbProviderFactory`. You can also provide complete convention records or register a Dapper type map directly:

```csharp
DbConventionTypeMap.Register(
    typeof(ProductOverview),
    DbCaseStyle.LowerSnakeCase,
    strictMode: false
);
```

Provider packages can implement `IDbConnectionProvider` and attach an `IDbProviderConfiguration` without introducing native driver types into the core session API. `Flowsy.Db.Unity.Postgres` uses this model to configure reusable `NpgsqlDataSource` instances:

```csharp
services.AddDatabases(options => options
    .UsePostgres(
        "Catalog",
        connectionString,
        postgres => postgres.MapComposite<PostalAddress>("postal_address"))
    .AsDefault());
```

Register custom Dapper type handlers with `DbServiceCollectionOptions.AddTypeHandler`. Replace `IDbWriteOperationDetector` or `IDbSessionSettingFormatter` in the service collection when provider or application policy requires different behavior.
