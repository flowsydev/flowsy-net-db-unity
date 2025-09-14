using Flowsy.Db.Unity.Test.Mock.Infrastructure.Database;
using Flowsy.Db.Unity.Test.Mock.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Npgsql;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Testcontainers.MySql;
using Testcontainers.PostgreSql;

namespace Flowsy.Db.Unity.Test.Mock;

public class ServiceHost : IDisposable, IAsyncDisposable
{
    private readonly IHost _host;
    private readonly PostgreSqlContainer _postgresContainer;
    private readonly MySqlContainer _mySqlContainer;
    private readonly Logger _logger;
    private bool _disposed;

    public ServiceHost()
    {
        _logger = CreateLogger();
        
        var containersConfig = Configuration.Instance.GetRequiredSection("Containers");

        Environment.SetEnvironmentVariable("DOCKER_HOST", containersConfig["DockerHost"]);

        var postgresContainer = containersConfig.GetRequiredSection("Postgres");
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage(postgresContainer["Image"])
            .WithDatabase(postgresContainer["Database"])
            .WithUsername(postgresContainer["Username"])
            .WithPassword(postgresContainer["Password"])
            .Build();

        var mySqlContainer = containersConfig.GetRequiredSection("MySql");
        _mySqlContainer = new MySqlBuilder()
            .WithImage(mySqlContainer["Image"])
            .WithDatabase(mySqlContainer["Database"])
            .WithUsername(mySqlContainer["Username"])
            .WithPassword(mySqlContainer["Password"])
            .Build();

        StartContainersAsync().Wait();

        _host = Host.CreateDefaultBuilder()
            .UseSerilog(_logger)
            .ConfigureServices(services =>
            {
                services.AddDatabases(options =>
                {
                    // Configure PostgreSQL as default database
                    options
                        .UseConnection(DbConnections.Postgres, _postgresContainer.GetConnectionString())
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
                            mappings:
                            [
                                new DbEnumMapping(typeof(ShoppingCartStatus), "shopping.shopping_cart_status"),
                                new DbEnumMapping(typeof(UserAccountStatus), "accounts.user_status")
                            ]
                        )
                        .WithDefault(DbCaseStyle.LowerSnakeCase);
                    
                    // Configure additional SQL Server for reports
                    options
                        .UseConnection(DbConnections.MySql, _mySqlContainer.GetConnectionString())
                        .WithProvider(DbProviderFamily.SqlServer, "MySqlConnector", MySqlConnectorFactory.Instance)
                        .WithLogLevel(LogLevel.Information)
                        .WithConventions()
                        .ForParameters(prefix: "p_", useNamedParameters: true)
                        .WithDefault(DbCaseStyle.LowerSnakeCase);
                    
                    // Type mapping for queries that return fields whose names are in lower_snake_case format.
                    // Databases like PostgreSQL and MySQL typically use this format by convention.
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
            })
            .UseSerilog()
            .Build();
        
        _host.StartAsync().Wait();
        
        MigrateDatabasesAsync().Wait();
    }

    ~ServiceHost()
    {
        Dispose(false);
    }
    
    public IServiceProvider Services => _host.Services;
    
    public IServiceScope CreateScope() => _host.Services.CreateScope();
    
    public AsyncServiceScope CreateAsyncScope() => _host.Services.CreateAsyncScope();

    private async Task MigrateDatabasesAsync()
    {
        await using var scope = CreateAsyncScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ServiceHost>>();
        try
        {
            var hub = scope.ServiceProvider.GetRequiredService<IDbConnectionHub>();
            
            foreach (var connectionKey in DbConnections.All)
            {
                var migrationPath = Path.Combine("Mock", "Infrastructure", "Database", "Scripts", connectionKey, "Migrations");
                if (!Directory.Exists(migrationPath))
                {
                    logger.LogWarning("Skipping migration for database '{ConnectionKey}', migration path not found: {MigrationPath}", connectionKey, migrationPath);
                    continue;
                }
                
                await using var db = await hub.CreateSessionAsync(connectionKey, DbConnectionUsage.Exclusive);
                
                var testResult = await db.QuerySingleAsync<int>("SELECT 1 as test_value");
                if (testResult != 1)
                    throw new Exception($"Database connection test failed for '{connectionKey}'");
                
                await db.MigrateAsync();
            }
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error durante la migración de bases de datos");
            throw;
        }
    }
    
    private Logger CreateLogger() => new LoggerConfiguration()
        .MinimumLevel.Information()
        .MinimumLevel.Override("Flowsy", LogEventLevel.Debug)
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .MinimumLevel.Override("System", LogEventLevel.Warning)
        .MinimumLevel.Override("Testcontainers", LogEventLevel.Warning)
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{ProviderName}] {Message:lj}{NewLine}{Exception}")
        .CreateLogger();

    private Task StartContainersAsync() => Task.WhenAll(
        _postgresContainer.StartAsync(),
        _mySqlContainer.StartAsync()
        );

    private async Task StopContainersAsync()
    {
        try
        {
            await Task.WhenAll(
                _postgresContainer.StopAsync(),
                _mySqlContainer.StopAsync()
                );
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error stopping containers");
        }
        finally
        {
            try
            {
                await Task.WhenAll(
                    _postgresContainer.DisposeAsync().AsTask(),
                    _mySqlContainer.DisposeAsync().AsTask()
                    );
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error disposing containers");
            }
        }
    }

    private async Task StopHostAsync()
    {
        try
        {
            await _host.StopAsync();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error stopping host");
        }
        finally
        {
            try
            {
                _host.Dispose();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error disposing host");
            }
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            StopContainersAsync().Wait();
            StopHostAsync().Wait();
        }
        
        _disposed = true;
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }

    private async ValueTask DisposeAsync(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            await StopContainersAsync();
            await StopHostAsync();   
        }
        
        _disposed = true;
    }
}