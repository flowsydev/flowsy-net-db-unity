# Result and Type Mapping

Register Dapper type maps when database column names follow a different naming style than your .NET models:

```csharp
options.MapTypes(
    DbCaseStyle.LowerSnakeCase,
    types: [typeof(Product), typeof(ProductOverview)]
);
```

Enum conventions can map .NET enums to provider-specific values or custom PostgreSQL enum types:

```csharp
.ForEnums(
    typeNameCaseStyle: DbCaseStyle.LowerSnakeCase,
    memberNameCaseStyle: DbCaseStyle.PascalCase,
    mappings: [new DbEnumMapping(typeof(ProductStatus), "store.product_status")]
)
```

`DateTime` values are sent as `DbType.DateTime2` with an unspecified kind to avoid provider-specific timezone interpretation. `DateTimeOffset` formatting follows the configured date-time convention.
