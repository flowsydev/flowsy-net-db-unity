using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Flowsy.Db.Unity.Configuration;

public class DbUnityServiceBuilder
{
    private readonly IServiceCollection _services;

    internal DbUnityServiceBuilder(IServiceCollection services)
    {
        services.TryAddSingleton<IReadOnlyCollection<ServiceDescriptor>>(new ReadOnlyCollection<ServiceDescriptor>(services));
        _services = services;
    }

    /// <summary>
    /// Adds the default connection factory to the service collection.
    /// This enables the use of the default connection factory when creating database connections.
    /// This is the default implementation of the IDbConnectionFactory interface.
    /// </summary>
    /// <returns></returns>
    public DbUnityServiceBuilder WithDefaultConnectionFactory()
    {
        _services.AddScoped<IDbConnectionFactory>(serviceProvider =>
        {
            var optionsSnapshot = serviceProvider.GetRequiredService<IOptionsSnapshot<DbConnectionOptions>>();
            return new DbConnectionFactory(optionsSnapshot);
        });
        return this;
    }
    
    /// <summary>
    /// Adds a custom connection factory to the service collection.
    /// This enables the use of a custom connection factory when creating database connections.
    /// </summary>
    /// <typeparam name="TService">
    /// The type of the service to be registered.
    /// </typeparam>
    /// <typeparam name="TImplementation">
    /// The type of the implementation to be registered.
    /// </typeparam>
    /// <returns>
    /// The current instance of the <see cref="DbUnityServiceBuilder"/> class.
    /// </returns>
    public DbUnityServiceBuilder WithConnectionFactory<TService, TImplementation>() 
        where TService : class, IDbConnectionFactory
        where TImplementation : DbConnectionFactory, TService
    {
        _services.AddScoped<TService, TImplementation>();
        return this;
    }
    
    /// <summary>
    /// Adds a custom connection factory to the service collection.
    /// This enables the use of a custom connection factory when creating database connections.
    /// </summary>
    /// <param name="implementationFactory">
    /// The factory method to create the implementation.
    /// </param>
    /// <typeparam name="TService">
    /// The type of the service to be registered.
    /// </typeparam>
    /// <typeparam name="TImplementation">
    /// The type of the implementation to be registered.
    /// </typeparam>
    /// <returns>
    /// The current instance of the <see cref="DbUnityServiceBuilder"/> class.
    /// </returns>
    public DbUnityServiceBuilder WithConnectionFactory<TService, TImplementation>(Func<IServiceProvider, TImplementation> implementationFactory) 
        where TService : class, IDbConnectionFactory
        where TImplementation : DbConnectionFactory, TService
    {
        _services.AddSingleton<TService>(implementationFactory);
        return this;
    }

    /// <summary>
    /// Adds the default database agent to the service collection.
    /// </summary>
    /// <param name="connectionKey">
    /// The connection key to use for resolving the connection options to be used by the agent.
    /// If not specified, the connection options marked as default will be used.
    /// </param>
    /// <returns>
    /// The current instance of the <see cref="DbUnityServiceBuilder"/> class.
    /// </returns>
    public DbUnityServiceBuilder WithDefaultAgent(string? connectionKey = null)
    {
        _services.AddTransient<IDbAgent>(serviceProvider =>
        {
            var optionsSnapshot = serviceProvider.GetRequiredService<IOptionsSnapshot<DbConnectionOptions>>();
            var options = optionsSnapshot.Get(connectionKey);
            var connectionScope = serviceProvider.GetRequiredService<IDbConnectionScope>();
            var logger = serviceProvider.GetService<ILogger<IDbAgent>>();
            return new DbAgent(options, connectionScope, logger);
        });
        return this;
    }
    
    /// <summary>
    /// Adds a custom database agent to the service collection.
    /// This enables the use of a custom database agent when performing database operations.
    /// </summary>
    /// <typeparam name="TService">
    /// The type of the service to be registered.
    /// </typeparam>
    /// <typeparam name="TImplementation">
    /// The type of the implementation to be registered.
    /// </typeparam>
    /// <returns>
    /// The current instance of the <see cref="DbUnityServiceBuilder"/> class.
    /// </returns>
    public DbUnityServiceBuilder WithAgent<TService, TImplementation>()
        where TService : class, IDbAgent
        where TImplementation : DbAgent, TService
    {
        _services.AddTransient<TService, TImplementation>();
        return this;
    }
    
    /// <summary>
    /// Adds a custom database agent to the service collection.
    /// This enables the use of a custom database agent when performing database operations.
    /// </summary>
    /// <param name="implementationFactory">
    /// The factory method to create the implementation.
    /// </param>
    /// <typeparam name="TService">
    /// The type of the service to be registered.
    /// </typeparam>
    /// <returns></returns>
    public DbUnityServiceBuilder WithAgent<TService>(Func<IServiceProvider, TService> implementationFactory)
        where TService : class, IDbAgent
    {
        _services.AddTransient(implementationFactory);
        return this;
    }

    /// <summary>
    /// Adds the default unit of work to the service collection.
    /// This enables the use of the default unit of work when performing database operations.
    /// This is the default implementation of the IDbUnitOfWork interface.
    /// </summary>
    /// <param name="connectionKey">
    /// The connection key to use for resolving the connection options to be used by the unit of work.
    /// If not specified, the connection options marked as default will be used.
    /// </param>
    /// <returns>
    /// The current instance of the <see cref="DbUnityServiceBuilder"/> class.
    /// </returns>
    public DbUnityServiceBuilder WithDefaultUnitOfWork(string? connectionKey = null)
    {
        _services.AddScoped<IDbUnitOfWork>(serviceProvider =>
        {
            var optionsSnapshot = serviceProvider.GetRequiredService<IOptionsSnapshot<DbConnectionOptions>>();
            var options = optionsSnapshot.Get(connectionKey);
            var connectionFactory = serviceProvider.GetRequiredService<IDbConnectionFactory>();
            var logger = serviceProvider.GetService<ILogger<DbUnitOfWork>>();
            return new DbUnitOfWork(options, connectionFactory, logger);
        });
        return this;
    }
    
    /// <summary>
    /// Adds a custom unit of work to the service collection.
    /// This enables the use of a custom unit of work when performing database operations.
    /// </summary>
    /// <typeparam name="TService">
    /// The type of the service to be registered.
    /// </typeparam>
    /// <typeparam name="TImplementation">
    /// The type of the implementation to be registered.
    /// </typeparam>
    /// <returns>
    /// The current instance of the <see cref="DbUnityServiceBuilder"/> class.
    /// </returns>
    public DbUnityServiceBuilder WithUnitOfWork<TService, TImplementation>()
        where TService : class, IDbUnitOfWork
        where TImplementation : DbUnitOfWork, TService
    {
        _services.AddScoped<TService, TImplementation>();
        return this;
    }
    
    /// <summary>
    /// Adds a custom unit of work to the service collection.
    /// This enables the use of a custom unit of work when performing database operations.
    /// </summary>
    /// <param name="implementationFactory">
    /// The factory method to create the implementation.
    /// </param>
    /// <typeparam name="TService">
    /// The type of the service to be registered.
    /// </typeparam>
    /// <returns>
    /// The current instance of the <see cref="DbUnityServiceBuilder"/> class.
    /// </returns>
    public DbUnityServiceBuilder WithUnitOfWork<TService>(Func<IServiceProvider, TService> implementationFactory)
        where TService : class, IDbUnitOfWork
    {
        _services.AddScoped(implementationFactory);
        return this;
    }
}