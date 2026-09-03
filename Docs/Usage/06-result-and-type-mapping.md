# Result And Type Mapping

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

Register global Dapper type handlers during database configuration:

```csharp
services.AddDatabases(options =>
{
    options.AddTypeHandler(new ProductCodeTypeHandler());
});
```

`DbDateOnlyTypeHandler` and `DbTimeOnlyTypeHandler` provide explicit handlers for modern .NET date and time values when a driver needs them. A registered type handler receives the original CLR value and takes precedence over inferred `DbType` conversion.
