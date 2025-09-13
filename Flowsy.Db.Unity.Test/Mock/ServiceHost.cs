using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Serilog;
using Serilog.Events;
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

        /*{
            using var connection = new NpgsqlConnection(PostgresConnectionString);
            connection.Migrate(Path.Combine("Mock", "Migrations", "Primary"));
        }

        {
            using var connection = new MySqlConnection(MySqlConnectionString);
            connection.Migrate(Path.Combine("Mock", "Migrations", "Secondary"));
        }*/

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Flowsy", LogEventLevel.Verbose)
            .WriteTo.Console()
            .CreateLogger();

        _host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                
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