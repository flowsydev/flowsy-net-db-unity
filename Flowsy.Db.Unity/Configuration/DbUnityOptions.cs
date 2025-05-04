using Flowsy.Db.Unity.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Flowsy.Db.Unity.Configuration;

public class DbUnityOptions
{
    private readonly IServiceCollection _services;
    private readonly DbUnityServiceBuilder _serviceBuilder;
    
    private readonly Dictionary<string, DbConnectionOptionsBuilder> _connectionOptionsBuilders = new();

    internal DbUnityOptions(IServiceCollection services, DbUnityServiceBuilder serviceBuilder)
    {
        _services = services;
        _serviceBuilder = serviceBuilder;
    }

    public DbConnectionOptionsBuilder UseConnection(string connectionKey)
    {
        if (string.IsNullOrWhiteSpace(connectionKey))
            throw new ArgumentException(Strings.ConnectionKeyCannotBeNullOrWhiteSpace, nameof(connectionKey));
        
        var options = new DbConnectionOptions(connectionKey);
        var builder = new DbConnectionOptionsBuilder(options);
        _connectionOptionsBuilders[connectionKey] = builder;
        return builder;
    }

    internal void RegisterServices()
    {
        _services.AddScoped<IDbConnectionScope, DbConnectionScope>();

        var unnamedOptionsRegistered = false;
        var index = -1;
        foreach (var (connectionKey, builder) in _connectionOptionsBuilders)
        {
            index++;
            var connectionOptions = builder.Build();
            
            if (!unnamedOptionsRegistered && (connectionOptions.Default || index == 0))
            {
                _services.AddOptions<DbConnectionOptions>().Configure(options =>
                {
                    connectionOptions.CopyTo(options);
                });
                unnamedOptionsRegistered = true;
            }
            _services.AddOptions<DbConnectionOptions>(connectionKey).Configure(options =>
            {
                connectionOptions.CopyTo(options);
            });
        }
    }
}