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
