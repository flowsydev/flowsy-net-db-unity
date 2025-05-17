# Flowsy Db Unity

This package is a wrapper around [Dapper](https://www.nuget.org/packages/Dapper) that allows you to unify the way your application
interacts with databases by defining a set of conventions for naming your database objects and invoking SQL commands.

## Concepts

### DbConventionSet

The `DbConventionSet` class puts together a set of conventions and exposes them as a set of properties:

- Provider: An object describing some features of the database provider associated with the set of conventions.
- Routines: A set of conventions for invoking stored procedures and functions.
- Parameters: A set of conventions for naming and passing parameters to routines and commands.
- Enums: A set of conventions for mapping enums to database types.
- DateTime: A set of conventions for mapping date and time values to database types.
- Commands: A set of conventions for SQL commands, such as flags and timeouts.

Besides the properties above, the `DbConventionSet` class defines a static property called `Default` that is used as a template for all the convention sets you create.
This default instance does not define any naming conventions like case style, prefixes or suffixes or any other relevant preferences, since it's meant to be a neutral base for all the other convention sets.

The mechanism for creating a new `DbConventionSet` is the `DbConventionSet.CreateBuilder` method, which returns a `DbConventionSetBuilder` instance.
This builder instance allows you to define the settings for the new convention set using a fluent API.

### IDbConnection Extension Methods

The library provides a set of extension methods for `IDbConnection` that allow you to execute SQL commands and retrieve data using a set of conventions.
In the previous examples, we used the `GetFromRoutine` method to retrieve data from a routine.
Along with this method, the library provides several other methods for executing SQL commands and retrieving data.

- `ExecuteRoutine`: Executes a routine (stored procedure or function) and returns the number of affected rows.
- `ExecuteScript`: Executes a SQL script and returns the number of affected rows.
- `ExecuteStatement`: Executes a SQL statement and returns the number of affected rows.
- `GetFromRoutine<T>`: Retrieves data from a routine and maps it to a collection of objects of type `T`.
- `GetFirstFromRoutine<T>`: Retrieves the first row from a routine and maps it to an object of type `T`.
- `GetFirstOrDefaultFromRoutine<T>`: Retrieves the first row from a routine and maps it to an object of type `T`, or returns `null` if no rows are found.
- `GetMultipleFromRoutine`: Retrieves multiple results from a routine.
- `GetSingleFromRoutine<T>`: Retrieves a single row from a routine and maps it to an object of type `T`.
- `GetSingleOrDefaultFromRoutine<T>`: Retrieves a single row from a routine and maps it to an object of type `T`, or returns `null` if no rows are found.
- `GetFromStatement<T>`: Retrieves data from a SQL statement and maps it to a collection of objects of type `T`.
- `GetFirstFromStatement<T>`: Retrieves the first row from a SQL statement and maps it to an object of type `T`.
- `GetFirstOrDefaultFromStatement<T>`: Retrieves the first row from a SQL statement and maps it to an object of type `T`, or returns `null` if no rows are found.
- `GetMultipleFromStatement`: Retrieves multiple results from a SQL statement.
- `GetSingleFromStatement<T>`: Retrieves a single row from a SQL statement and maps it to an object of type `T`.
- `GetSingleOrDefaultFromStatement<T>`: Retrieves a single row from a SQL statement and maps it to an object of type `T`, or returns `null` if no rows are found.

All the methods above accept an optional `DbConventionSet` parameter that allows you to specify a custom set of conventions for the command execution.
Finally, all the above methods have an async version that returns a `Task` instead of a result: {MethodName}Async.

### DbConnectionOptions

The `DbConnectionOptions` class is used to configure the options for a given database connection.
It allows you to specify the following options:

- ConnectionKey: A unique key for each set of connection options you create.
- Provider: The database provider associated with the connection options.
- ConnectionString: The connection string to be used for the database connection.
- Default: A value indicating whether the connection is the default connection for the application.
- Conventions: An optional set of conventions to be used for the connection (`DbConventionSet`).
- LogLevel: The log level to be used for logging operations for the database connection.

This class is used by services like `IDbConnectionFactory`, `IDbConnectionScope`, `IDbAgent` and `IDbUnitOfWork`
to get connections and execute commands that comply with the conventions of your choice.

### IDbConnectionFactory

The `IDbConnectionFactory` interface is used to obtain database connections identified by a unique key.
Consumers of this service must dispose of the connections when they are no longer needed.

### IDbConnectionScope

The `IDbConnectionScope` interface is meant to be used in scenarios where you need to manage the lifetime of a database connection within a given scope.
Consumers of this service do not need to dispose of the connections, as they are automatically disposed of when the scope is disposed.

### IDbAgent

The `IDbAgent` interface is used to execute SQL commands and retrieve data from a database using the options defined by a given `DbConnectionOptions` instance.
This interface defines a set of methods matching the extension methods for `IDbConnection` described above.
The difference is that the methods of this interface require less parameters, since they are meant to use the options defined by the associated `DbConnectionOptions` instance to execute the commands.

The default implementation of this interface is `DbAgent`, which uses services such as `DbConnectionOptions` and `IDbConnectionScope` to obtain a connection and execute the commands.

### IDbUnitOfWork

The `IDbUnitOfWork` interface defines a set of methods that allow you to perform database operations within a transactional context.

- `BeginWork`: Starts a new unit of work.
- `Involve`: Involves an action or participant service in the unit of work.
- `CompleteWork`: Completes the unit of work and commits the transaction.
- `CompleteWorkAsync`: Asynchronously completes the unit of work and commits the transaction.
- `DiscardWork`: Discards the unit of work and rolls back the transaction.
- `DiscardWorkAsync`: Asynchronously discards the unit of work and rolls back the transaction.

This service is meant to be used within a scope where you need to perform multiple database operations and ensure that they are all committed or rolled back as a single unit of work.
If the `IDbUnitOfWork` goes out of scope without being **explicitly completed**, the transaction will be rolled back automatically.

The default implementation of this interface is `DbUnitOfWork`, which uses services such as `DbConnectionOptions` and `IDbConnectionScope` to obtain database connections.

### IDbUnitOfWorkParticipant

The `IDbUnitOfWorkParticipant` interface allows you to define a service that can be involved in a unit of work.
The members of this interface are:

- UnitOfWork: The unit of work instance that the participant is involved in.
- IsParticipating: A value indicating whether the participant is currently participating in a unit of work.
- Join(IDbUnitOfWork): Joins the participant to a unit of work.
- Leave: Leaves the current unit of work.
- BelongsTo(IDbUnitOfWork): A value indicating whether the participant belongs to a given unit of work.

## Basic Usage

### Default Conventions

Even though the built-in `DbConvention.Default` instance may work well for most cases, you can set it to a new value that better fits your needs.
For instance, if your application has to interact with several databases, you can create a default set of conventions before creating the specific ones for each database.
This way, you can define your basic preferences once and then override only specific settings for each database.

```csharp
DbConventionSet.Default = DbConventionSet.CreateBuilder()
    .UseDefaultCaseStyle(CaseStyle.LowerSnakeCase) // Use this case style for all database objects (can be overridden for specific objects)
    .ForParameters()
    .UsePrefix("p_") // In your database all parameters are prefixed with "p_", in your code you just use C# style property names (PascalCase)
    .ForEnums()
    .UseValueFormat(DbEnumFormat.Name) // Send the enum member name to the database instead of the ordinal value.
    .UseNames(CaseStyle.UpperSnakeCase) // Send the enum member name in UPPER_SNAKE_CASE
    .Build();
```

### Custom Conventions

Suppose your application has to interact with two different databases: PostgreSQL and MySQL.
This is how you can create a custom convention set for each database:

#### PostgreSQL

```csharp
var postgres = new DbProviderDescriptor(DbProviderFamily.Postgres);
var postgresConventions = DbConventionSet.CreateBuilder(postgres) // Builders are always based on the DbConventionSet.Default instance
    .ForRoutines()
    .UseFunctions(prefix: "fun_", useNamedParameters: true) // Functions will be invoked if no routine type is specified, they are prefixed with "fun_" and must be called with named parameters
    .UseProcedureNames(prefix: "pro_") // You can still call procedures when you need to and they are prefixed with "pro_".
    .ForEnums()
    .UseMapping<SomeEnum>("some_schema.some_enum") // Your SomeEnum runtime type is mapped to the some_schema.some_enum database type.
    .Build();

IDbConnection connection = new NpgsqlConnection("your-connection-string");
var customers = connection.GetFromRoutine<Customer>( 
    "crm.get_customers",
        // crm is the schema name, get_customers is the routine name without the prefix, it will be translated to "crm.fun_get_customers".
        // If you think it's clearer for you and your team, you can include the prefix, it won't be duplicated.
    new
    {
        SearchTerm = "@example.com" // It will be translated to "p_search_term".
    }, 
    conventions: postgresConventions // If no conventions are specified, the values fron DbConventionSet.Default will be used.
    );

// According to the conventions defined above, the routine will be called as follows:
// SELECT * FROM crm.fun_get_customers(p_search_term => '@example.com');
```

#### MySQL

```csharp
var mySql = new DbProviderDescriptor(DbProviderFamily.MySql);
var mySqlConventions = DbConventionSet.CreateBuilder(mySql) // Builders are always based on the DbConventionSet.Default instance
    .ForRoutines()
    .UseProcedures(prefix: "pro_") // Stored procedures will be invoked and they are prefixed with "pro_".
    .ForEnums()
    .UseNames(CaseStyle.UpperSnakeCase) // Enum member names will be sent in UPPER_SNAKE_CASE.
    .Build();

IDbConnection connection = new MySqlConnection("your-connection-string");
var customers = connection.GetFromRoutine<Customer>( 
    "get_customers",
        // get_customers is the routine name without the prefix, it will be translated to "pro_get_customers".
        // If you think it's clearer for you and your team, you can include the prefix, it won't be duplicated.
    new
    {
        SearchTerm = "@example.com"
    }, 
    conventions: postgresConventions // If no conventions are specified, the values fron DbConventionSet.Default will be used.
    );

// For stored procedures, the final command will be configured as follows:
// CommandText: pro_get_customers
// CommandType: StoredProcedure
// Parameters: p_search_term = '@example.com'
```

### Mapping Queries to Types

This package includes the `DbConventionTypeMap` class which implements the `SqlMapper.ITypeMap` interface from Dapper.
The `DbConventionTypeMap` class allows you to set type mappings based on the naming conventions of your choice.

Let's suppose you want to map the results of some database queries to objects of some types defined by your application.

```csharp
// Use lower_snake_case when mapping results to the Customer type and enable strict mode (last parameter set to true).
var customerType = typeof(Customer);
DbConventionTypeMap.Register(customerType, CaseStyle.LowerSnakeCase, true);
```

Strict mode forces all columns from database queries to match members of the mapped types.
The default value for this parameter is false, but you can set it to true in your development and testing environments to ensure that your queries are always returning the expected columns.

In the previous example we are mapping a single type, but you could use reflection to select a collection of types and map them all at once using the same conventions.

Let's suppose you define an interface named `IReadModel` and make all your read models implement it.

```csharp
var readModelInterfaceType = typeof(IReadModel);
var readModelTypes = Assembly.GetExecutingAssembly()
    .GetTypes()
    .Where(t => readModelInterfaceType.IsAssignableFrom(t) && t is {IsAbstract: false, IsInterface: false});

foreach (var readModelType in readModelTypes)
{
    DbConventionTypeMap.Register(readModelType, CaseStyle.LowerSnakeCase, true);
}
```

In the previous example we are using a fictitious `IReadModel` interface to target all read models in our executing assembly,
but you can use the name of your choice or any custom logic to select the types you want to map.

## Dependency Injection

Although you can use the extensions methods for `IDbConnection` directly or manually instantiate services like `IDbAgent` and `IDbUnitOfWork`,
for aplications based on .NET's dependency injection system, you may prefer to take advantage of the corresponding features provided by this library.

### Registering the Services

Let's suppose you have a web application and you want to register the services provided by this library.

#### Single Database

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddDbUnity(options => 
    {
        options
            .UseConnection("Default") // Give a key to the connection options.
            .WithProvider(DbProviderFamily.Postgres, "Npgsql", NpgsqlFactory.Instance) // Specify the database provider (Npgsql library).
            .WithConnectionString("your-connection-string") // Specify the connection string.
            .WithConventions()
            .UseDefaultCaseStyle(CaseStyle.LowerSnakeCase) // Use lower_snake_case for all database objects.
            .ForRoutines()
            .UseFunctions(prefix: "fun_", useNamedParameters: true) // Functions will be invoked if no routine type is specified, they are prefixed with "fun_" and must be called with named parameters)
            .UseProcedureNames(prefix: "pro_") // You can still call procedures when you need to and they are prefixed with "pro_".
            .ForEnums()
            .UseNames(CaseStyle.UpperSnakeCase) // Send the enum member name in UPPER_SNAKE_CASE to the database instead of the ordinal value.
            .UseMapping<SomeEnum>("some_schema.some_enum"); // Your SomeEnum runtime type is mapped to the some_schema.some_enum database type.
    })
    .WithDefaultConnectionFactory() // Use the default connection factory.
    .WithDefaultAgent() // Use the default agent.
    .WithDefaultUnitOfWork(); // Use the default unit of work.

// Register more services...

var app = builder.Build();

// Activate services...

app.Run();
```

#### Multiple Databases

##### Service Registration
```csharp
//////////////////////////////////////////////////////////////////////
// Service registration
//////////////////////////////////////////////////////////////////////
var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddDbUnity(options => 
    {
        // Configure default conventions (DbConventionSet.Default).
        options.UseDefaultConventions(conventions => 
        {
            conventions
                .UseDefaultCaseStyle(CaseStyle.LowerSnakeCase) // Use this case style for all database objects (can be overridden for specific objects)
                .ForParameters()
                .UsePrefix("p_") // In your database all parameters are prefixed with "p_", in your code you just use C# style property names (PascalCase)
                .ForEnums()
                .UseNames(CaseStyle.UpperSnakeCase); // Send the enum member name in UPPER_SNAKE_CASE instead of the ordinal value.
        });
    
        // Configure your first database connection.
        options
            .UseConnection("Primary") // Give a unique key to your first connection.
            .AsDefault() // Set this connection as the default one.
            .WithProvider(DbProviderFamily.Postgres, "Npgsql", NpgsqlFactory.Instance) // Specify the database provider (Npgsql library).
            .WithConnectionString("your-connection-string") // Specify the connection string.
            .WithConventions()
            .ForRoutines()
            .UseFunctions(prefix: "fun_", useNamedParameters: true) // Functions will be invoked if no routine type is specified, they are prefixed with "fun_" and must be called with named parameters)
            .UseProcedureNames(prefix: "pro_") // You can still call procedures when you need to and they are prefixed with "pro_".
            .ForEnums()
            .UseMapping<SomeEnum>("some_schema.some_enum") // Your SomeEnum runtime type is mapped to the some_schema.some_enum database type.
    
        // Configure your second database connection.
        options
            .UseConnection("Secondary") // Give a unique key to your second connection.
            .WithProvider(DbProviderFamily.MySql, "MySql.Data.MySqlClient", MySqlClientFactory.Instance) // Specify the database provider (MySql.Data library).
            .WithConnectionString("your-connection-string") // Specify the connection string.
            .WithConventions()
            .ForRoutines()
            .UseProcedures(prefix: "pro_") // Stored procedures will be invoked and they are prefixed with "pro_".
            .ForEnums()
            .UseNames(CaseStyle.UpperSnakeCase); // Enum member names will be sent in UPPER_SNAKE_CASE.
  
        options.MapTypes(o =>
        {
            // Map your read models to the database tables.
            // Here you can apply any logic to select the types you want to map using a given naming convention.
            var readModelInterfaceType = typeof(IReadModel);
            var readModelTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => readModelInterfaceType.IsAssignableFrom(t) && t is {IsAbstract: false, IsInterface: false});
        
            o.AddTypeGroup(CaseStyle.LowerSnakeCase, readModelTypes.ToArray());
            o.StrictMode = true; // Strict mode is disabled by default. When enabled, all columns from database queries must match the members of the mapped types.
        });
    })
    .WithDefaultConnectionFactory() // Use the default implementation of IDbConnectionFactory. You can use WithConnectionFactory<S, I>() instead to register your own implementation.
    .WithDefaultAgent("Primary") // Key of the connection to associate with the default agent. If not specified, the default connection will be used.
    .WithDefaultUnitOfWork("Primary") // Key of the connection to associate with the default unit of work. If not specified, the default connection will be used.
    .WithAgent<IDbSecondaryUnitOfWork, DbSecondaryUnitOfWork>() // Register a custom agent for the secondary connection.
    .WithUnitOfWork<IDbSecondaryUnitOfWork, DbSecondaryUnitOfWork>(); // Register a custom unit of work for the secondary connection.

// Register more services...

var app = builder.Build();

// Activate services...

app.Run();
```

#### Agent and Unit of Work for Additional Connections

The options for each connection are registered using the `IOptions<T>` pattern.
This means that you can inject the `DbConnectionOptions` service in your classes and get the options for a given connection using the key you specified.

##### Fictitious IDbSecondaryAgent
```csharp
public interface IDbSecondaryAgent : IDbAgent;

public class DbSecondaryAgent : DbAgent, IDbSecondaryAgent
{
    public DbSecondaryAgent(IOptionsSnapshot<DbConnectionOptions> optionsSnapshot, ILogger<DbSecondaryAgent> logger)
        : base(optionsSnapshot.Get("Secondary"), logger)
    {
    }
}
```

##### Fictitious IDbSecondaryUnitOfWork
```csharp
public interface IDbSecondaryUnitOfWork : IDbUnitOfWork;

public class DbSecondaryUnitOfWork : DbUnitOfWork, IDbSecondaryUnitOfWork
{
    public DbSecondaryUnitOfWork(IOptionsSnapshot<DbConnectionOptions> optionsSnapshot, IDbConnectionScope connectionScope, ILogger<DbSecondaryUnitOfWork> logger)
        : base(optionsSnapshot.Get("Secondary"), connectionScope, logger)
    {
    }
}
```


### Using the Services

#### Primary Connection: Retrieving Data
```csharp
public record CustomerBySearchTermQuery(string? SearchTerm);

public static class CustomerBySearchTermQueryHandler
{
    public static async Task<IEnumerable<Customer>> HandleAsync(
        CustomerBySearchTermQuery query,
        IDbAgent agent,
        CancellationToken cancellationToken = default
        )
    {
        // SELECT * FROM crm.fun_customer_get_by_search_term(p_search_term => '...');
        var customers = await agent.GetFromRoutineAsync<Customer>(
            "crm.customer_get_by_search_term",
            new
            {
                SearchTerm = query.SearchTerm
            },
            cancellationToken
            );

        return customers;
    }
}
```

#### Primary Connection: Mutating Data
```csharp
public record CreateCustomerCommand(string Name, string Email, IEnumerable<ContactChannel> ContactChannels);

public static class CreateCustomerCommandHandler
{
    public static async Task HandleAsync(
        CreateCustomerCommand command,
        IDbUnitOfWork unitOfWork,
        IDbAgent agent,
        CancellationToken cancellationToken = default
        )
    {
        unitOfWork.BeginWork();
        
        unitOfWork.Involve(agent); // Involve the agent in the unit of work.
        
        var customerId = Guid.NewGuid();
        
        /// SQL Command
        /// CommandText: crm.pro_customer_create
        /// CommandType: StoredProcedure
        /// Parameters: p_customer_id = '...', p_name = '...', p_email = '...'
        await agent.ExecuteRoutineAsync(
            "crm.customer_create",
            DbRoutineType.StoredProcedure,
            new
            {
                CustomerId = customerId,
                Name = command.Name,
                Email = command.Email
            },
            cancellationToken
            );
        
        foreach (var contactChannel in command.ContactChannels)
        {
            /// SQL Command
            /// CommandText: crm.pro_customer_add_contact_channel
            /// CommandType: StoredProcedure
            /// Parameters: p_customer_id = '...', p_type = '...', p_endpoint = '...'       
            await agent.ExecuteRoutineAsync(
                "crm.customer_add_contact_channel",
                DbRoutineType.StoredProcedure,
                new
                {
                    CustomerId = customerId,
                    Type = contactChannel.Type, // Enum value (e.g. Email, Phone, etc.)
                    Endpoint = contactChannel.Endpoint // The actual value (e.g. email address, phone number, etc.)
                },
                cancellationToken
                );
        }
        
        // If an exception is thrown before this line, the unit of work will go
        // out of scope and the transaction will be rolled back automatically.
        unitOfWork.CompleteWork(); // Commit the transaction.
    }
}
```

#### Secondary Connection: Retrieving Data
```csharp
public record ProductBySearchTermQuery(string? SearchTerm);

public static class ProductBySearchTermQueryHandler
{
    public static async Task<IEnumerable<Product>> HandleAsync(
        ProductBySearchTermQuery query,
        IDbSecondaryAgent agent,
        CancellationToken cancellationToken = default
        )
    {
        // SQL Command
        // CommandText: pro_product_get_by_search_term
        // CommandType: StoredProcedure
        // Parameters: p_search_term = '...'
        var products = await agent.GetFromRoutineAsync<Product>(
            "product_get_by_search_term",
            new
            {
                SearchTerm = query.SearchTerm
            },
            cancellationToken
            );

        return products;
    }
}
```

#### Secondary Connection: Mutating Data
```csharp
public record AddProductToQuoteCommand(Guid QuoteId, Guid ProductId, int Quantity);

public static class AddProductToQuoteCommandHandler
{
    public static async Task HandleAsync(
        AddProductToQuoteCommand command,
        IDbSecondaryUnitOfWork unitOfWork,
        IDbSecondaryAgent agent,
        CancellationToken cancellationToken = default
        )
    {
        decimal total = 0;
        decimal taxes = 0;
        decimal grandTotal = 0;
        
        // Perform validation, enforce business rules and calculate totals...
        
        unitOfWork.BeginWork();
        
        unitOfWork.Involve(agent); // Involve the agent in the unit of work.
        
        /// SQL Command
        /// CommandText: pro_quote_add_product
        /// CommandType: StoredProcedure
        /// Parameters: p_quote_id = '...', p_product_id = '...', p_quantity = 1
        await agent.ExecuteRoutineAsync(
            "quote_add_product",
            new
            {
                QuoteId = command.QuoteId,
                ProductId = command.ProductId,
                Quantity = command.Quantity
            },
            cancellationToken
            );
        
        /// SQL Command
        /// CommandText: pro_quote_update_total
        /// CommandType: StoredProcedure
        /// Parameters: p_quote_id = '...', p_total = ..., p_taxes = ..., p_grand_total = ...
        await agent.ExecuteRoutineAsync(
            "quote_update_total",
            new
            {
                QuoteId = command.QuoteId,
                Total = total // The new total amount of the quote.
                Taxes = taxes // The new taxes amount of the quote.
                GrandTotal = grandTotal // The new grand total amount of the quote.
            },
            cancellationToken
            );
        
        // If an exception is thrown before this line, the unit of work will go
        // out of scope and the transaction will be rolled back automatically.
        unitOfWork.CompleteWork(); // Commit the transaction.
    }
}
```

#### Explicit Agents and Units Of Work

If you want to be more explicit, you can register a custom agent and unit of work
for the primary connection as well, instead of using the default ones:

```csharp
builder.Services
    .AddDbUnity(options => 
    {
        // Configure the options...
    })
    .WithDefaultConnectionFactory()
    .WithAgent<IDbPrimaryUnitOfWork, DbPrimaryUnitOfWork>()
    .WithUnitOfWork<IDbPrimaryUnitOfWork, DbPrimaryUnitOfWork>();
    .WithAgent<IDbSecondaryUnitOfWork, DbSecondaryUnitOfWork>()
    .WithUnitOfWork<IDbSecondaryUnitOfWork, DbSecondaryUnitOfWork>();
```

This way, the code in your services will explicitly indicate which agent and unit of work are consuming.