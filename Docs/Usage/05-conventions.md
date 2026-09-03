# Naming And Parameter Conventions

Conventions control object names, routine calls, command settings, parameters, enums, and date-time values.

```csharp
.WithConventions()
.ForRoutines(DbRoutineType.StoredFunction, DbCaseStyle.LowerSnakeCase)
.ForParameters(prefix: "p_", useNamedParameters: true)
.ForCommands(timeout: 60)
.WithDefault(DbCaseStyle.LowerSnakeCase)
```

Available case styles include lower snake case, upper snake case, camel case, and Pascal case. Provider descriptors determine details such as statement parameter prefixes, schema support, array support, and routine formatting.

## Explicit Parameter And Enum Mappings

Keep exceptional database names outside domain models by mapping them in the connection conventions:

```csharp
.ForParameters(parameters => parameters
    .WithDefault(DbCaseStyle.LowerSnakeCase, "p_")
    .Map<CreateProduct, Guid>(x => x.ProductId, "p_catalog_key"))
.ForEnums(new DbEnumConvention(
    DbEnumValueFormat.Name,
    Mappings:
    [new DbEnumMapping<ProductStatus>(null, [(ProductStatus.AwaitingReview, "awaiting-review")])]))
```
