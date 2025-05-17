using System.Reflection;
using Flowsy.Core;
using Flowsy.Db.Unity.Test.Mock.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Npgsql;
using Serilog;
using Testcontainers.MySql;
using Testcontainers.PostgreSql;

namespace Flowsy.Db.Unity.Test.Mock;

public class ServiceHost : IDisposable
{
    private readonly PostgreSqlContainer _postgresContainer;
    private readonly MySqlContainer _mySqlContainer;
    private readonly IHost _host;
    private bool _disposed;

    public IServiceProvider ServiceProvider => _host.Services;
    public string PostgresConnectionString { get; }
    public string MySqlConnectionString { get; }

    public ServiceHost()
    {
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

        Task.WhenAll(_postgresContainer.StartAsync(), _mySqlContainer.StartAsync()).Wait();
        
        var postgresConnectionStringBuilder = new NpgsqlConnectionStringBuilder(_postgresContainer.GetConnectionString())
        {
            IncludeErrorDetail = true
        };
        PostgresConnectionString = postgresConnectionStringBuilder.ToString();
        MySqlConnectionString = _mySqlContainer.GetConnectionString();

        {
            using var connection = new NpgsqlConnection(PostgresConnectionString);
            connection.Migrate(Path.Combine("Mock", "Migrations", "Primary"));
        }

        {
            using var connection = new MySqlConnection(MySqlConnectionString);
            connection.Migrate(Path.Combine("Mock", "Migrations", "Secondary"));
        }

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();

        _host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services
                    .AddDbUnity(options =>
                    {
                        options.UseDefaultConventions(conventions =>
                        {
                            conventions
                                .UseDefaultCaseStyle(CaseStyle.LowerSnakeCase)
                                .ForParameters()
                                .UsePrefix("p_")
                                .ForEnums()
                                .UseValueFormat(DbEnumFormat.Name)
                                .UseNames(CaseStyle.UpperSnakeCase);
                        });
                        
                        options
                            .UseConnection("Primary")
                            .AsDefault()
                            .WithProvider(DbProviderFamily.Postgres, "Npgsql", NpgsqlFactory.Instance)
                            .WithConnectionString(postgresConnectionStringBuilder.ToString())
                            .WithConventions()
                            .ForRoutines()
                            .UseFunctions(prefix: "fun_")
                            .ForEnums()
                            .UseMapping<Currency>("kernel.currency");

                        options
                            .UseConnection("Secondary")
                            .WithProvider(DbProviderFamily.MySql, "MySql.Data.MySqlClient", MySqlClientFactory.Instance)
                            .WithConnectionString(MySqlConnectionString)
                            .WithConventions()
                            .ForRoutines()
                            .UseProcedures(prefix: "pro_");
                        
                        options.MapTypes(o =>
                        {
                            var readModelInterfaceType = typeof(IReadModel);
                            var readModelTypes = Assembly.GetExecutingAssembly()
                                .GetTypes()
                                .Where(t => readModelInterfaceType.IsAssignableFrom(t) && t is {IsAbstract: false, IsInterface: false});
                            
                            o.AddTypeGroup(CaseStyle.LowerSnakeCase, readModelTypes.ToArray());
                            o.StrictMode = true;
                        });
                    })
                    .WithDefaultConnectionFactory()
                    .WithDefaultAgent()
                    .WithDefaultUnitOfWork()
                    .WithAgent<IDbPrimaryAgent, DbPrimaryAgent>()
                    .WithUnitOfWork<IDbPrimaryUnitOfWork, DbPrimaryUnitOfWork>()
                    .WithAgent<IDbSecondaryAgent, DbSecondaryAgent>()
                    .WithUnitOfWork<IDbSecondaryUnitOfWork, DbSecondaryUnitOfWork>();
                
                services.AddScoped<IPrimaryCustomerRepository, PrimaryCustomerRepository>();
            })
            .UseSerilog()
            .Build();
        
        _host.StartAsync().Wait();
    }
    
    ~ServiceHost() => Dispose(disposing: false);

    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            Task.WhenAll(_postgresContainer.StopAsync(), _mySqlContainer.StopAsync()).Wait();
            _host.StopAsync().Wait();
            _host.Dispose();
        }

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}