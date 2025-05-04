using Flowsy.Core;
using Flowsy.Db.Unity.Conventions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MySql.Data.MySqlClient;
using Npgsql;
using Serilog;
using Testcontainers.MySql;
using Testcontainers.PostgreSql;

namespace Flowsy.Db.Unity.Test.Mock;

public class ServiceHost : IDisposable
{
    private readonly IHost _host;
    private readonly PostgreSqlContainer _postgresContainer;
    private readonly MySqlContainer _mySqlContainer;
    private bool _disposed;

    public IServiceProvider ServiceProvider => _host.Services;

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
                        options
                            .UseConnection("Primary")
                            .AsDefault()
                            .WithProvider(DbProviderFamily.Postgres, "Npgsql", NpgsqlFactory.Instance)
                            .WithConnectionString(_postgresContainer.GetConnectionString())
                            .WithConventions()
                            .ForRoutines()
                            .UseType(DbRoutineType.Function)
                            .UseFunctionPrefix("fun_")
                            .UseFunctionSuffix("_fun")
                            .UseProcedurePrefix("pro_")
                            .UseProcedureSuffix("_pro")
                            .UseCaseStyle(CaseStyle.LowerSnakeCase)
                            .ForParameters()
                            .UsePrefix("p_")
                            .UseSuffix("_p")
                            .UseCaseStyle(CaseStyle.LowerSnakeCase)
                            .ForEnums()
                            .UseFormat(DbEnumFormat.Name)
                            .UseCaseStyle(CaseStyle.PascalCase)
                            .UseMapping<Gender>("kernel.gender");

                        options
                            .UseConnection("Secondary")
                            .AsDefault()
                            .WithProvider(DbProviderFamily.MySql, "MySql.Data.MySqlClient", MySqlClientFactory.Instance)
                            .WithConnectionString(_mySqlContainer.GetConnectionString())
                            .WithConventions()
                            .ForRoutines()
                            .UseProcedurePrefix("pro_")
                            .UseProcedureSuffix("_pro")
                            .UseCaseStyle(CaseStyle.LowerSnakeCase)
                            .ForParameters()
                            .UsePrefix("p_")
                            .UseSuffix("_p")
                            .UseCaseStyle(CaseStyle.LowerSnakeCase)
                            .ForEnums()
                            .UseFormat(DbEnumFormat.Name)
                            .UseCaseStyle(CaseStyle.PascalCase)
                            .UseMapping<Gender>("gender");
                    })
                    .WithDefaultConnectionFactory()
                    .WithDefaultAgent()
                    .WithDefaultUnitOfWork()
                    .WithAgent<IDbPrimaryAgent, DbPrimaryAgent>()
                    .WithUnitOfWork<IDbPrimaryUnitOfWork, DbPrimaryUnitOfWork>()
                    .WithAgent<IDbSecondaryAgent, DbSecondaryAgent>()
                    .WithUnitOfWork<IDbSecondaryUnitOfWork, DbSecondaryUnitOfWork>();
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
            _postgresContainer.StopAsync().Wait();
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