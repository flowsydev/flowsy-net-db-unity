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

## Session Setting Restoration

Custom `IDbSessionSettingFormatter` implementations can supply `ReadStatement` and `RestoreStatementFactory` on `DbSessionSettingCommand`. Supply both properties: the session reads a scalar value before applying the command and uses the factory to build its restoration SQL. Validate identifiers and quote captured values appropriately for the provider.

Existing commands constructed with only `ApplyStatement` and `CleanupStatement` remain supported and use their explicit cleanup SQL; their formatter is responsible for restoring the intended state.
