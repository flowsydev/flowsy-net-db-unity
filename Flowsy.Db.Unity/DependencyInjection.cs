using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Flowsy.Db.Unity;

/// <summary>
/// Provides extension methods for configuring database services in the dependency injection container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds and configures database services in the dependency injection container.
    /// </summary>
    /// <param name="services">
    /// The service collection where database services will be registered.
    /// </param>
    /// <param name="configure">
    /// Action to configure database service options, including connections and conventions.
    /// </param>
    /// <returns>
    /// The updated service collection to allow fluent configuration.
    /// </returns>
    public static IServiceCollection AddDatabases(this IServiceCollection services, Action<DbServiceCollectionOptions> configure)
    {
        var options = new DbServiceCollectionOptions(services);
        configure(options);

        var validConnecitonConfigurationBuilders = options
            .ConnectionConfigurationBuilders
            .Where(b => b.IsValid)
            .ToList();
        
        var defaultConnectionConfigurationBuilder = validConnecitonConfigurationBuilders.FirstOrDefault(b => b.Default);
        if (defaultConnectionConfigurationBuilder is not null)
        {
            services.AddOptions<DbConnectionConfiguration>().Configure(config =>
            {
                defaultConnectionConfigurationBuilder.Build(config);
            });
        }
        
        foreach (var connectionConfigurationBuilder in validConnecitonConfigurationBuilders)
        {
            services.AddOptions<DbConnectionConfiguration>(connectionConfigurationBuilder.ConnectionKey).Configure(config =>
            {
                connectionConfigurationBuilder.Build(config);
            });
        }

        if (!options.ConnectionFactoryRegistered)
        {
            services.AddSingleton<IDbConnectionFactory>(serviceProvider =>
            {
                var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<DbConnectionConfiguration>>();
                return new DbConnectionFactory(optionsMonitor);
            });
        }
        
        if (!options.SessionFactoryRegistered)
            services.AddScoped<IDbSessionFactory, DbSessionFactory>();
        
        services.AddScoped<IDbConnectionHub, DbConnectionHub>();
        
        return services;
    }
}