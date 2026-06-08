# Naming and Parameter Conventions

Conventions control object names, routine calls, command settings, parameters, enums, and date-time values.

```csharp
.WithConventions()
.ForRoutines(DbRoutineType.StoredFunction, DbCaseStyle.LowerSnakeCase)
.ForParameters(prefix: "p_", useNamedParameters: true)
.ForCommands(timeout: 60)
.WithDefault(DbCaseStyle.LowerSnakeCase)
```

Available case styles include lower snake case, upper snake case, camel case, and Pascal case. Provider descriptors determine details such as statement parameter prefixes, schema support, array support, and routine formatting.
