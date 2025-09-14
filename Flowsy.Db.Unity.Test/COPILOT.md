# Copilot Instructions

## Overview

- The general specifications for the .NET solution are defined in the `COPILOT.md` file located in the root folder of the repository.
- This file provides specific instructions for creating test classes that validate CRUD operations on a fictional eCommerce application using different database providers (Postgres and MySql).

## Scenarios
- The `Mock/Infrastructure/Database/DbConnections.cs` file defines the connection keys for different database providers.
    - MySql (MySqlConnector)
    - Oracle (Oracle.ManagedDataAccess.Core)
    - Postgres (Npgsql)
    - Sqlite (Microsoft.Data.Sqlite)
    - SqlServer (Microsoft.Data.SqlClient)
- The test classes in `Scenarios/[ConnectionKey]` target the fictional eCommerce application using the corresponding database provider:
  - Postgres
  - MySql
- The test classes are designed to validate CRUD operations on different tables:
  - `security.user_account`
  - `shopping.product_category`
  - `shopping.product`
  - `shopping.shopping_cart`
- The test classes should be named according to the pattern `S[SS][ConnectionKey][Entity]Test` where:
  - `[SS]` is a two-digit code representing a sequential number for the test scenario within (e.g., `01`, `02`, etc.).
  - `[ConnectionKey]` is a connection key defined in the `DbConnections` class (e.g., `Postgres`, `MySql`, etc.).
  - `[Entity]` is the entity being tested (e.g., `ProductCategory`, `Product`, `UserAccount`, `ShoppingCart`).
- The test methods within a test class are named according to the pattern `T[TT]_Should_[Action]` where:
  - `[TT]` is a two-digit code representing a sequential number for the test method (e.g., `01`, `02`, etc.).
  - `[Action]` describes the action being tested (e.g., `CreateProductCategory`, `ReadProduct`, `UpdateUserAccount`, `DeleteShoppingCart`, etc.).
  - The test methods should cover the CRUD operations using the `IDbConnectionHub` and `IDbSession` services. 
  - The test methods should use dependency injection to obtain services from the `ServieHost` class.
  - The test methods should cover the following methods from the `IDbSession` service:
    - `QueryAsync<T>`
    - `QueryFromRoutineAsync<T>`
    - `QueryFirstAsync<T>`
    - `QueryFirstFromRoutineAsync<T>`
    - `QueryFirstOrDefaultAsync<T>`
    - `QueryFirstOrDefaultFromRoutineAsync<T>`
    - `QuerySingleAsync<T>`
    - `QuerySingleFromRoutineAsync<T>`
    - `QuerySingleOrDefaultAsync<T>`
    - `QuerySingleOrDefaultFromRoutineAsync<T>`
    - `QueryMultipleAsync`
    - `QueryMultipleFromRoutineAsync`
    - `ExecuteAsync`
    - `ExecuteRoutineAsync`
    - `ExecuteScriptAsync`
    - `BeginTransactionAsync`
    - `CommitTransactionAsync`
    - `RollbackTransactionAsync`
  - The previous methods from the `IDbSession` service receive parameters as dynamic objects or anonymous types where:
    - The property names match the parameter names defined in the database routines.
    - The property names are expected to be in `PascalCase`, as expected for any other C# object.
    - The property names are formated as specified when configuring dependency injection in `ServiceHost`.
    - The property types are compatible with the parameter types defined in the database routines.
    - The properties for result types `T` are mapped as specified when configuring dependency injection in `ServiceHost`.
  - The test methods should use the `Shouldly` library for assertions, in order to ensure that the operations succeed and the data is correctly stored and retrieved.

### Dependency Injection Configuration Example
```csharp
// services is of type IServiceCollection

services.AddDatabases(options =>
{
    // Configure PostgreSQL as default database
    options
        .UseConnection(DbConnections.Postgres, _postgresContainer.GetConnectionString())
        .AsDefault()
        .WithProvider(DbProviderFamily.Postgres, "Npgsql", NpgsqlFactory.Instance)
        .WithLogLevel(LogLevel.Information)
        .WithMigrations(Path.Combine("Mock", "Infrastructure", "Database", "Scripts", DbConnections.Postgres, "Migrations"))
        .WithConventions()
        .ForRoutines(DbRoutineType.StoredFunction)
        .ForParameters(prefix: "p_", useNamedParameters: true) // A property named "UserId" maps to a parameter named "p_user_id"
        .ForEnums(
            typeNameCaseStyle: DbCaseStyle.LowerSnakeCase,
            memberNameCaseStyle: DbCaseStyle.PascalCase,
            mappings:
            [
                new DbEnumMapping(typeof(ShoppingCartStatus), "shopping.shopping_cart_status"),
                new DbEnumMapping(typeof(UserAccountStatus), "security.user_status")
            ]
        )
        .WithDefault(DbCaseStyle.LowerSnakeCase);
    
    // Column names in lower_snake_case map to properties in PascalCase
    // A column named "product_category_id" maps to a property named "ProductCategoryId"
    // A column named "creation_instant" maps to a property named "CreationInstant"
    options.MapTypes(DbCaseStyle.LowerSnakeCase, types:
    [
        typeof(ProductCategoryOverview),
        typeof(ProductCategoryDetail),
        typeof(ProductOverview),
        typeof(ProductDetail),
        typeof(UserAccountOverview),
        typeof(UserAccountDetail),
        typeof(ShoppingCartOverview),
        typeof(ShoppingCartDetail),
    ]);
});
```

### Test Class Examples

#### Postgres - Product Category
```csharp
/// <summary>
/// Given a PostgreSQL database for an eCommerce application
/// When the application connects to the database and performs CRUD operations on product categories
/// Then the operations should succeed and the data should be correctly stored and retrieved
/// </summary>
[Collection(Collections.Postgres), Order(1)]
public class C01S01PostgresProductCategoryTest
{
    private const string ConnectionKey = DbConnections.Postgres;
    
    private readonly ServiceHost _host;
    private readonly ITestOutputHelper _output;
    
    public C01S01PostgresProductCategoryTest(ServiceHost host, ITestOutputHelper output)
    {
        _host = host;
        _output = output;
    }
    
    [Theory, Order(1)]
    [InlineData("electronics", "Electronics", "Devices and gadgets")]
    [InlineData("fashion", "Fashion", "Clothing and accessories")]
    [InlineData("home_kitchen", "Home & Kitchen", "Household items and kitchenware")]
    [InlineData("sports", "Sports", "Sporting goods and outdoor equipment")]
    public async Task T01_Should_Create_Product_Category(string code, string name, string description)
    {
        // Arrange
        await using var scope = _host.CreateAsyncScope();
        
        var connectionHub = scope.ServiceProvider.GetService<IDbConnectionHub>();
        connectionHub.ShouldNotBeNull();
        
        await using var db = await connectionHub.CreateSessionAsync(ConnectionKey);
        
        // Act
        Exception? exception = null;
        try
        {
            _output.WriteLine("Creating product category: {0} | {1} | {2}", code, name, description);
            
            await db.ExecuteRoutineAsync(
                "shopping.product_category_create",
                new
                {
                    Code = code, 
                    Name = name,
                    Description = description,
                    CreationInstant = Clock.GetTimestamp()
                }
                );
            
            _output.WriteLine("Product category created successfully: {0} | {1} | {2}", code, name, description);
        }
        catch (Exception ex)
        {
            exception = ex;
            _output.WriteLine(ex.ToString());
        }

        // Assert
        exception.ShouldBeNull();
    }
}
```

### Database Scripts
- The database scripts for creating the necessary tables and routines are located in the `Mock/Infrastructure/Database/Scripts/[ConnectionKey]/Migrations` folder.
- The scripts are automatically executed from the `ServiceHost` class during the initialization of the test environment.
- The scripts should be compliant with the `Evolve` migration tool and should be named according to the expected pattern:
  - Versioned: `V[VVV]__[Description].sql` for creating or altering tables
    - `[VVV]` is a three-digit version number (e.g., `001`, `002`, etc.).
    - `[Description]` is a brief description of the script (e.g., `create_product_category_table`, `create_product_routines`, etc.).
  - Repeatable: `R__[Description].sql` for creating routines or views
    - [Description] is a brief description of the routine or view (e.g., `product_category_create`, `best_selling_product`, etc.).
    - The scripts should be idempotent and should not fail if executed multiple times.
- The scripts should include the creation of the following objects:
  - Tables corresponding to the `[Entity]Detail` records defined in the `Mock/Model` folder:
    - `security.user_account` and its indexes: `V001__create_user_account_table.sql`
    - `shopping.product_category` and its indexes: `V002__create_product_category_table.sql`
    - `shopping.product` and its indexes: `V003__create_product_table.sql`
    - `shopping.shopping_cart` and its indexes: `V004__create_shopping_cart_table.sql`
  - Stored procedures or functions for CRUD operations on the tables.
- Each table should be created with the following specifications:
  - All table and column names should be in `lower_snake_case`.
  - Hava a primary key named `[table_name]_id` of type `UUID`.
  - Have a unique constraint on the `code` column, if applicable.
  - Have a `created_instant` column of type `TIMESTAMP` with time zone and without a default value.
  - Have a `last_mutation_instant` column of type `TIMESTAMP` with time zone and without a default value.
