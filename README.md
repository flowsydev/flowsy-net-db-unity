# Flowsy DB Unity

A .NET library that provides advanced functionality for accessing SQL databases with a configurable set of conventions and options for naming and formatting objects like tables, columns, routines, and parameters.

## Installation

```bash
dotnet add package Flowsy.Db.Unity
```

## Developer Documentation

See the [usage guide](Docs/Usage/README.md) for focused documentation about configuration, sessions, queries, routines, conventions, type mapping, migrations, transactions, and extensibility.

## Features

- **Multiple database providers**: Compatible with PostgreSQL, SQL Server, MySQL, Oracle, and SQLite
- **Configurable conventions**: Customize column mapping, routine nomenclature, parameter usage, and data types
- **Intelligent connection management**: Automatic connection lifecycle management with transaction support
- **Database sessions**: Unified interface for CRUD operations with integrated logging
- **Routine support**: Execution of stored procedures and functions with specific conventions
- **Dapper integration**: High-level wrapper over [Dapper](https://www.learndapper.com/) with additional functionality
- **Evolve migrations**: Database migration support using [Evolve](https://evolve-db.netlify.app/)
- **Dependency injection**: Fluent configuration through .NET's DI container

## Core Architecture

### Core Services

#### `IDbConnectionFactory` / `DbConnectionFactory`

Singleton service that creates database connections based on configurations identified by unique keys.

#### `IDbConnectionHub` / `DbConnectionHub`

Scoped service that manages database connections and controls their lifecycle, including opening, closing, and resource disposal.

#### `IDbSession` / `DbSession`

Represents a database session that allows performing query, execution, and transaction operations using Dapper as the underlying engine.

## Basic Configuration

### 1. Service Registration

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using Npgsql;
using Flowsy.Db.Unity;

// Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabases(options =>
{
    // Configure PostgreSQL as default database
    options
        .UseConnection("MainDatabase", connectionString)
        .AsDefault()
        .WithProvider(DbProviderFamily.Postgres, "Npgsql", NpgsqlFactory.Instance)
        .WithLogLevel(LogLevel.Debug)
        .WithMigrations("/path/to/migrations")
        .WithConventions()
        .ForRoutines(DbRoutineType.StoredFunction)
        .ForParameters(prefix: "p_", useNamedParameters: true)
        .ForEnums(
            typeNameCaseStyle: DbCaseStyle.LowerSnakeCase, 
            memberNameCaseStyle: DbCaseStyle.PascalCase,
            mappings: [
                new DbEnumMapping(typeof(ShoppingCartStatus), "shopping.shopping_cart_status"),
                new DbEnumMapping(typeof(UserStatus), "accounts.user_status")
            ]
        )
        .WithDefault(DbCaseStyle.LowerSnakeCase);

    // Type mapping for queries that return fields whose names are in lower_snake_case format.
    // Databases like PostgreSQL and MySQL typically use this format by convention.
    options.MapTypes(DbCaseStyle.LowerSnakeCase, types: [
        typeof(Product),
        typeof(ProductOverview),
        typeof(ShoppingCart),
        typeof(ShoppingCartOverview),
        typeof(Category),
        typeof(UserAccount)
    ]);

    // Configure additional SQL Server for reports
    options
        .UseConnection("ReportsDatabase", reportsConnectionString)
        .WithProvider(DbProviderFamily.SqlServer, "Microsoft.Data.SqlClient", SqlClientFactory.Instance)
        .WithLogLevel(LogLevel.Information)
        .WithConventions()
        .ForParameters(prefix: "P_", useNamedParameters: true)
        .WithDefault(DbCaseStyle.PascalCase);

    // Type mapping for queries that return fields whose names are in PascalCase format.
    // Databases like SQL Server typically use this format by convention.
    options.MapTypes(DbCaseStyle.PascalCase, types: [
        typeof(SalesReport),
        typeof(MonthlyReport),
        typeof(CustomerSummary)
    ]);
});

var app = builder.Build();
// ...
app.Run();
```

### 2. Usage in Controllers or Services

```csharp
using Microsoft.Extensions.Logging;
using Flowsy.Db.Unity;

public class ProductService
{
    private readonly IDbConnectionHub _connectionHub;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IDbConnectionHub connectionHub, ILogger<ProductService> logger)
    {
        _connectionHub = connectionHub;
        _logger = logger;
    }

    public async Task<IEnumerable<Product>> GetActiveProductsAsync()
    {
        // Use default connection (MainDatabase)
        using var session = await _connectionHub.CreateSessionAsync("MainDatabase");
        
        return await session.QueryAsync<Product>(
            "SELECT product_id, name, price FROM products WHERE is_active = @p_is_active",
            new { IsActive = true }
        );
    }

    public async Task<Product?> GetProductByIdAsync(Guid productId)
    {
        using var session = await _connectionHub.CreateSessionAsync("MainDatabase");
        
        return await session.QueryFirstOrDefaultAsync<Product>(
            "SELECT * FROM products WHERE product_id = @p_product_id",
            new 
            {
                ProductId = productId
            }
        );
    }

    public async Task<Guid> CreateProductAsync(CreateProductRequest request)
    {
        using var session = await _connectionHub.CreateSessionAsync("MainDatabase");
        
        // Use stored procedure
        var result = await session.QuerySingleFromRoutineAsync<Guid>(
            "create_product", // select * from create_product(p_name => @p_name, p_category_id => @p_category_id, p_price => @p_price); 
            new 
            { 
                Name = request.Name,
                CategoryId = request.CategoryId,
                Price = request.Price 
            }
        );
        
        return result;
    }
}
```

### 3. Multiple Connection Usage

```csharp
using Flowsy.Db.Unity;

public class ReportService
{
    private readonly IDbConnectionHub _connectionHub;

    public ReportService(IDbConnectionHub connectionHub)
    {
        _connectionHub = connectionHub;
    }

    public async Task<SalesReport> GenerateReportAsync(DateTime from, DateTime to)
    {
        // Use specific connection for reports
        using var session = await _connectionHub.CreateSessionAsync("ReportsDatabase");
        
        return await session.QueryFirstFromRoutineAsync<SalesReport>(
            "generate_sales_report", // EXEC generate_sales_report @FromDate = @P_FromDate, @P_ToDate = @P_ToDate;
            new 
            {
                FromDate = from,
                ToDate = to
            }
        );
    }
}
```

### Transactions

```csharp
using Flowsy.Db.Unity;

public async Task TransferFundsAsync(Guid fromAccountId, Guid toAccountId, decimal amount)
{
    using var session = await _connectionHub.CreateSessionAsync("MainDatabase");
    
    session.BeginTransaction();
    
    try
    {
        // Debit source account
        await session.ExecuteAsync(
            "UPDATE accounts SET balance = balance - @p_amount WHERE account_id = @p_account_id",
            new 
            {
                Amount = amount,
                AccountId = fromAccountId
            }
        );
        
        // Credit destination account
        await session.ExecuteAsync(
            "UPDATE accounts SET balance = balance + @p_amount WHERE account_id = @p_account_id",
            new 
            {
                Amount = amount,
                AccountId = toAccountId
            }
        );
        
        await session.CommitTransactionAsync();
    }
    catch(Exception exception)
    {
        // Error logging
        // _logger.LogError(exception, "Error during fund transfer");
        
        // No need to call RollbackTransactionAsync() explicitly
        // IDbSession performs rollback automatically when exiting the
        // code block where the session was created (TransferFundsAsync method)
        throw;
    }
}
```

### Script Execution

```csharp
using Flowsy.Db.Unity;

// For migrations or database initialization
using var session = await _connectionHub.CreateSessionAsync("MainDatabase");

// Execute individual script
await session.ExecuteScriptAsync("path/to/script.sql");

// Execute script folder
await session.ExecuteScriptAsync("path/to/scripts/");

// Execute migrations according to Evolve conventions (versioned and repeatable)
await session.MigrateAsync();
```

## IDbSession Methods

### Query Operations

#### Queries that return multiple results
- **`QueryAsync<T>()`** - Returns multiple results as `IEnumerable<T>`
- **`QueryFromRoutineAsync<T>()`** - Returns multiple results from routine as `IEnumerable<T>`

#### Queries that return the first result
- **`QueryFirstAsync<T>()`** - First result (throws exception if no results found)
- **`QueryFirstFromRoutineAsync<T>()`** - First result from routine (throws exception if no results found)
- **`QueryFirstOrDefaultAsync<T>()`** - First result or default value if no results found
- **`QueryFirstOrDefaultFromRoutineAsync<T>()`** - First result from routine or default value

#### Queries that return a single result
- **`QuerySingleAsync<T>()`** - Single result (throws exception if no results or multiple results found)
- **`QuerySingleFromRoutineAsync<T>()`** - Single result from routine (throws exception if no results or multiple found)
- **`QuerySingleOrDefaultAsync<T>()`** - Single result or default value (throws exception if multiple found)
- **`QuerySingleOrDefaultFromRoutineAsync<T>()`** - Single result from routine or default value

#### Queries with multiple result sets
- **`QueryMultipleAsync()`** - Returns multiple result sets in a single query
- **`QueryMultipleFromRoutineAsync()`** - Returns multiple result sets from routine

### Execution Operations

#### Command execution
- **`ExecuteAsync()`** - Executes SQL statement (INSERT, UPDATE, DELETE) and returns affected rows
- **`ExecuteRoutineAsync()`** - Executes stored procedure or function and returns affected rows

### Script Operations
- **`ExecuteScriptAsync()`** - Executes SQL script from file or folder

### Transaction Operations
- **`BeginTransaction()`** - Starts a transaction
- **`CommitTransactionAsync()`** - Commits the current transaction
- **`RollbackTransactionAsync()`** - Rolls back the current transaction

## Method Parameters

All Dapper wrapper methods accept the following parameters:
- `string statement` or `string routineName`: SQL statement or routine name to execute.
- `dynamic? parameters = null`: Dynamic parameters (anonymous object) for the query or routine.
- `CancellationToken cancellationToken = default`: Token to cancel the asynchronous operation.


## Logging

All operations include structured logging with unique session and operation identifiers:

```
[SESSION:{SessionId} > OP:{OperationId}] {Message}
[SESSION:{SessionId} > OP:{OperationId}] {Message}
```
