# Copilot - Instructions

## IDbConnectionFactory and DbConnectionFactory
- The `IDbConnectionFactory` interface represents a service that creates database connections.
- The `DbConnectionFactory` class implements the `IDbConnectionFactory` interface and uses `DbConnectionConfiguration` objects to create database connections.
- Each database to connect to must have a unique key (string) that identifies it, which is recommended to use in `PascalCase` format.

## IDbConnectionHub and DbConnectionHub
- The `IDbConnectionHub` interface represents a service that manages database connections.
- The `DbConnectionHub` class implements the `IDbConnectionHub` interface and uses an `IDbConnectionFactory` to create database connections through their unique key.
- An `IDbConnectionHub` service is intended to be registered as a `Scoped` service in the dependency injection container.
- An `IDbConnectionHub` service must manage connection closing and resource disposal when exiting its scope.

## Conventions
- The `Conventions` folder of the `Flowsy.Db.Unity` project contains classes that define conventions for database interaction.
- Conventions can be configured for a specific database connection and include:
    - Mapping query columns to class properties.
    - Naming for stored procedures and functions.
    - Use of parameters in queries and stored procedures.
    - Use of `enum` types
    - Use of `DateTime` and `DateTimeOffset` types
    - Command execution options.

## IDbSession and DbSession
- The `IDbSession` interface represents a database session created by an `IDbConnectionHub` service
- The `DbSession` class implements the `IDbSession` interface and uses Dapper to interact with the database.
- The `DbSession` class includes a method called `BuildCommandDefinition` that builds command definitions for Dapper based on naming conventions and parameter usage.
- The `IDbSession` interface and `DbSession` class must include methods that act as wrappers over Dapper:
    - ExecuteAsync
    - QueryAsync<T>
    - QueryFirstAsync<T>
    - QueryFirstOrDefaultAsync<T>
    - QueryMultipleAsync
    - QuerySingleAsync<T>
    - QuerySingleOrDefaultAsync<T>
- In addition to wrapper methods over Dapper, the `IDbSession` interface and `DbSession` class must include versions of the same methods focused on executing database routines (stored procedures or functions):
    - ExecuteRoutineAsync
    - QueryFromRoutineAsync<T>
    - QueryFirstFromRoutineAsync<T>
    - QueryFirstOrDefaultFromRoutineAsync<T>
    - QueryMultipleFromRoutineAsync
    - QuerySingleFromRoutineAsync<T>
    - QuerySingleOrDefaultFromRoutineAsync<T>
- The `IDbSession` interface and `DbSession` class are defined as `partial` to be organized in multiple files, one for each group of methods or functionality group:
    - `IDbSession`
        - IDbSession.Execute.cs
        - IDbSession.ExecuteRoutine.cs
        - IDbSession.Query.cs
        - IDbSession.QueryFromRoutine.cs
        - IDbSession.QueryFirst.cs
        - IDbSession.QueryFirstFromRoutine.cs
        - IDbSession.QueryMultiple.cs
        - IDbSession.QueryMultipleFromRoutine.cs
        - IDbSession.QuerySingle.cs
        - IDbSession.QuerySingleFromRoutine.cs
    - `DbSession`
        - DbSession.Execute.cs
        - DbSession.ExecuteRoutine.cs
        - DbSession.Query.cs
        - DbSession.QueryFromRoutine.cs
        - DbSession.QueryFirst.cs
        - DbSession.QueryFirstFromRoutine.cs
        - DbSession.QueryMultiple.cs
        - DbSession.QueryMultipleFromRoutine.cs
        - DbSession.QuerySingle.cs
        - DbSession.QuerySingleFromRoutine.cs
- The methods of the `IDbConnectionHub` interface and `DbConnectionHub` class that are wrappers over Dapper receive parameters for the underlying SQL command as a dynamic object, which is expected to respect the C# `PascalCase` convention for its properties.
- The methods of the `IDbSession` interface and `DbSession` class must be ordered as shown in the previous lists.
- Each operation of the `IDbSession` services must register logs with `ILogger` for:
    - Operation start
    - Operation success
    - Operation error
- Logs of `IDbSession` services must include context properties and comply with the following format:
    - Message start: `[ SESSION:{SessionId} > OP:{OperationId} ]`
    - Short and descriptive message of the operation
    - Include identifiers for each operation (command, SQL statement, routine, script, etc.), so that the start and success or failure of it can be tracked.
    - The start message of an operation must include parameters and details of the operation, if any.
    - The success message of an operation must include relevant details of the result.
    - The error message of an operation must include error details.

## XML Documentation

The following rules must be followed for XML documentation in the `Flowsy.Db.Unity` project:
- All documentation must be in English.
- It must be clear and concise.
- It must describe the purpose and behavior of classes, methods, properties and other members.
- It must include usage examples when relevant.
- It must follow C# style conventions.
- Methods of type `First`, `FirstOrDefault`, `Single` and `SingleOrDefault` must be documented to indicate:
    - First: that they throw an exception if no results are found.
    - FirstOrDefault: that they return the default value if no results are found.
    - Single: that they throw an exception if no results are found or if multiple results are found.
    - SingleOrDefault: that they return the default value if no results are found, but throw an exception if multiple results are found.

### Example
```csharp
/// <summary>
/// Represents a database session that allows performing query operations and transactions.
/// </summary>
public interface IDbSession
{
    /// <summary>
    /// Unique key that identifies the database connection.
    /// </summary>
    string ConnectionKey { get; }
    
    /// <summary>
    /// Unique identifier of the database session.
    /// </summary>
    Guid SessionId { get; }
    
    /// <summary>
    /// Tracking identifier of the database session.
    /// </summary>
    string TrackingId { get; }

    /// <summary>
    /// Indicates whether the database session is participating in a transaction.
    /// </summary>
    bool InTransaction { get; }
    
    /// <summary>
    /// Starts a transaction in the database session.
    /// </summary>
    /// <param name="isolationLevel">
    /// Transaction isolation level. Default is <see cref="IsolationLevel.ReadCommitted"/>.
    /// </param>
    void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    
    /// <summary>
    /// Completes the current transaction in the database session.
    /// </summary>
    void CommitTransaction();
    /// <summary>
    /// Completes the current transaction in the database session asynchronously.
    /// </summary>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of completing the current transaction in the database session.
    /// </returns>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Reverts the current transaction in the database session.
    /// </summary>
    void RollbackTransaction();
    
    /// <summary>
    /// Reverts the current transaction in the database session asynchronously.
    /// </summary>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of reverting the current transaction in the database session.
    /// </returns>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Performs a database query and returns an enumerable of results.
    /// </summary>
    /// <param name="statement">
    /// SQL statement to execute.
    /// </param>
    /// <param name="parameters">
    /// Optional parameters for the query, which can be used to avoid SQL injections.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <typeparam name="T">
    /// Type of expected results from the query.
    /// </typeparam>
    /// <returns>
    /// A task that represents the asynchronous database query operation,
    /// </returns>
    Task<IEnumerable<T>> QueryAsync<T>(
        string statement,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
        );
    
    /// <summary>
    /// Performs a query to a stored procedure or function in the database and returns an enumerable of results.
    /// </summary>
    /// <param name="routineName">
    /// Name of the stored procedure or function to execute.
    /// </param>
    /// <param name="parameters">
    /// Optional parameters for the query.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <typeparam name="T">
    /// Type of expected results from the query.
    /// </typeparam>
    /// <returns>
    /// A task that represents the asynchronous database query operation,
    /// </returns>
    Task<IEnumerable<T>> QueryFromRoutineAsync<T>(
        string routineName,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
        );

    /// <summary>
    /// Performs a query to a stored procedure or function in the database and returns an enumerable of results.
    /// </summary>
    /// <param name="routineName">
    /// Name of the stored procedure or function to execute.
    /// </param>
    /// <param name="routineType">
    /// Type of routine (stored procedure or function).
    /// </param>
    /// <param name="parameters">
    /// Optional parameters for the query.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <typeparam name="T">
    /// Type of expected results from the query.
    /// </typeparam>
    /// <returns>
    /// A task that represents the asynchronous database query operation,
    /// </returns>
    Task<IEnumerable<T>> QueryFromRoutineAsync<T>(
        string routineName,
        DbRoutineType routineType,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
        );
    
    /// <summary>
    /// Executes a stored procedure or function in the database.
    /// </summary>
    /// <param name="routineName">
    /// Name of the stored procedure or function to execute.
    /// </param>
    /// <param name="parameters">
    /// Dynamic parameters that will be passed to the stored procedure or function.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of executing the stored procedure or function.
    /// </returns>
    Task<int> ExecuteRoutineAsync(
        string routineName,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
        );

    /// <summary>
    /// Executes a stored procedure or function in the database.
    /// </summary>
    /// <param name="routineName">
    /// Name of the stored procedure or function to execute.
    /// </param>
    /// <param name="routineType">
    /// Type of routine: stored procedure or function.
    /// </param>
    /// <param name="parameters">
    /// Dynamic parameters that will be passed to the stored procedure or function.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of executing the stored procedure or function.
    /// </returns>
    Task<int> ExecuteRoutineAsync(
        string routineName,
        DbRoutineType routineType,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
        );
}
```

## Automated Testing
- The `Flowsy.Db.Unity.Test` project contains unit and integration tests for the services and conventions of the `Flowsy.Db.Unity` project.
- Tests must cover:
    - Expected behavior of `IDbConnectionFactory`, `IDbConnectionHub` and `IDbSession` services.
    - Error and exception handling.
    - Database interaction through `Testcontainers`.
- Use the xUnit testing framework.
- Use `Shouldly` for assertions.
- Organize tests in the `Scenarios` folder in collections, classes and methods with descriptive, friendly, non-technical names that clearly indicate the purpose of each test.
- Use the `Xunit.Extensions.Ordering` extension for test ordering.
- Use `ITestOutputHelper` and its extensions in `Extensions/TestOutputExtensions.cs` to log during test execution.
- Use `Theory` and `InlineData` for parameterized tests when appropriate.
- Apply the AAA (Arrange, Act, Assert) pattern and each method should have a single purpose and be independent.
- Test names must comply with the following conventions:
    - Classes: `[CollectionName]/[ScenarioName]Test.cs` (use `[X][NN]` prefix for folder and class name if order is relevant)
    - Methods: `T[NN][Given_When_Then]`
- Cover all supported databases:
    - Postgres (Npgsql)
    - SQL Server (Microsoft.Data.SqlClient)
    - MySQL (MySqlConnector)
    - Oracle (Oracle.ManagedDataAccess.Core)
    - SQLite (Microsoft.Data.Sqlite)
- Use `[DbProvider]ServiceHostFactory` services to create `ServiceHost` services configured with services for each database provider.
- Use `Flowsy.Db.Unity.DependencyInjection.AddDatabases` to configure connections and map read models with desired database conventions.
- Use Test Collections to run tests in parallel with different database providers.
- Use a single `ServiceHost` at assembly level
    - Run database migrations from the `ServiceHost` after starting Testcontainers containers and before running tests.
    - Use it in all test methods to create scopes and obtain `IDbConnectionHub` services from which to create `IDbSession` sessions.
- Implement different test scenarios based on the simulated shopping cart domain (described below).
    - Use domain terminology in test class and method names.
    - Use `IDbSession` for all database operations.
    - Use different alternatives for CRUD operations (direct SQL statements, stored procedures and functions).
- There must be a folder called `Mock`, with the following content:
    - Domain
        - Contains a simple simulation of a shopping cart domain with elements like (but not limited to):
            - ProductOverview
            - ShoppingCartDetail
            - ShoppingCartOverview
            - ShoppingCartStatus
        - Use `record` with primary constructor for:
            - Read Models.
            - Objects with parameters to execute SQL statements and routines through `IDbSession`.
        - Use `enum` for cases like ShoppingCartStatus, UserStatus, etc.
    - Database
        - DbConnections.cs: Defines the unique keys (ConnectionKey) of the databases used in tests:
            - Postgres
            - MySql
            - SqlServer
            - Oracle
            - Sqlite
        - DbExtendedConnectionFactory.cs
            - Inherits from `DbConnectionFactory`
            - Implements `IDbConnectionFactory`
            - When a Postgres connection is required, creates it using `NpgsqlDataSource` and maps C# `enum` types to Postgres `enum` types.
            - For the rest of the providers, uses the base implementation of `DbConnectionFactory`.
        - DbPostgresEnumNameTranslator.cs
            - Implements `INpgsqlNameTranslator`
            - Translates C# `enum` type names to Postgres `enum` type names.
        - Migrations
            - Includes versioned and repeatable migration scripts for databases used in tests.
            - Each database folder must comply with the following rules:
                - Be named with the unique database key (ConnectionKey) in `PascalCase` format.
                    - Postgres
                    - MySql
                    - SqlServer
                    - Oracle
                    - Sqlite
                - Naming style:
                    - All database providers, except SqlServer:
                        - Everything in lower_snake_case
                        - Prefixes:
                            - Views: vw_
                            - Functions: fn_
                            - Stored procedures: sp_
                            - Function and stored procedure parameters: p_
                    - SqlServer:
                        - Everything in PascalCase
                        - Prefixes:
                            - Views: Vw_
                            - Functions: Fn_
                            - Stored procedures: Sp_
                            - Function and stored procedure parameters: P_
                - Follow `Evolve` conventions plus others defined here in `Versioned` and `Repeatable` subfolders, whether this tool is used in tests or not.
                    - `Versioned`. Versioned migration scripts:
                        - Name: V[NNN]__[script_description].sql.
                        - Create schemas, tables, and indexes.
                        - One script per schema or group of related tables.
                        - Table names must be in singular.
                        - All tables must include a primary key (table name + id)
                        - Primary keys must be of UUID/GUID type.
                        - Examples:
                            - V001__create_sales_schema.sql
                            - V002__create_products_table.sql
                            - V003__create_categories_table.sql
                            - V004__create_users_table.sql
                    - `Repeatable`. Repeatable migration scripts:
                        - Name: R__[script_description].sql.
                        - Create views, stored procedures and functions.
                        - One script per view, stored procedure or function.
                        - In the same script, first drop (if they exist) and then create the objects.
                        - Script names must include a prefix indicating the type of object they create:
                            - Views: `R__vw_`
                            - Stored procedures: `R__sp_`
                            - Functions: `R__fn_`
                        - Examples:
                            - R__fn_create_shopping_cart.sql
                            - R__sp_get_open_shopping_cart_by_user_account_id.sql

## IMPORTANT!
- Oracle tests are temporarily disabled due to Testcontainers container performance issues.
- Investigation and resolution of these issues is required before reactivating the tests.
- Until then, this class only contains a test that always passes to avoid CI/CD failures.
- Although tests are disabled, Oracle continues to be considered in the implementation of all scenarios, to ensure future compatibility.

## CHANGELOG.md
- Located at the root of the repository.
- Follow the [Keep a Changelog](https://keepachangelog.com/) format
- Follow the recommendations of [Semantic Versioning](https://semver.org/)
- Include an `Unreleased` section for changes that have not yet been released.
- Each version should include a list of changes categorized as:
    - Added: for new features.
    - Changed: for changes in existing features.
    - Deprecated: for features that will be removed in future versions.
    - Removed: for features that have been removed in this version.
    - Fixed: for bug fixes.
    - Security: for security-related improvements.
- Analyze commit messages to automatically generate the changelog.
