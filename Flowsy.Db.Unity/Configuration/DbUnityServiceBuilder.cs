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

    public DbUnityServiceBuilder WithDefaultConnectionFactory()
    {
        _services.AddScoped<IDbConnectionFactory>(serviceProvider =>
        {
            var optionsSnapshot = serviceProvider.GetRequiredService<IOptionsSnapshot<DbConnectionOptions>>();
            return new DbConnectionFactory(optionsSnapshot);
        });
        return this;
    }
    
    public DbUnityServiceBuilder WithConnectionFactory<TService, TImplementation>() 
        where TService : class, IDbConnectionFactory
        where TImplementation : DbConnectionFactory, TService
    {
        _services.AddScoped<TService, TImplementation>();
        return this;
    }
    
    public DbUnityServiceBuilder WithConnectionFactory<TService, TImplementation>(Func<IServiceProvider, TImplementation> implementationFactory) 
        where TService : class, IDbConnectionFactory
        where TImplementation : DbConnectionFactory, TService
    {
        _services.AddSingleton<TService>(implementationFactory);
        return this;
    }

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
    
    public DbUnityServiceBuilder WithAgent<TService, TImplementation>()
        where TService : class, IDbAgent
        where TImplementation : DbAgent, TService
    {
        _services.AddTransient<TService, TImplementation>();
        return this;
    }
    
    public DbUnityServiceBuilder WithAgent<TService, TImplementation>(Func<IServiceProvider, TImplementation> implementationFactory)
        where TService : class, IDbAgent
        where TImplementation : DbAgent, TService
    {
        _services.AddTransient<TService>(implementationFactory);
        return this;
    }

    public DbUnityServiceBuilder WithDefaultUnitOfWork(string? connectionKey = null)
    {
        _services.AddScoped<IDbUnitOfWork>(serviceProvider =>
        {
            var optionsSnapshot = serviceProvider.GetRequiredService<IOptionsSnapshot<DbConnectionOptions>>();
            var options = optionsSnapshot.Get(connectionKey);
            var logger = serviceProvider.GetService<ILogger<DbUnitOfWork>>();
            return new DbUnitOfWork(options, serviceProvider, logger);
        });
        return this;
    }
    
    public DbUnityServiceBuilder WithUnitOfWork<TService, TImplementation>()
        where TService : class, IDbUnitOfWork
        where TImplementation : DbUnitOfWork, TService
    {
        _services.AddScoped<TService, TImplementation>();
        return this;
    }
    
    public DbUnityServiceBuilder WithUnitOfWork<TService, TImplementation>(Func<IServiceProvider, TImplementation> implementationFactory)
        where TService : class, IDbUnitOfWork
        where TImplementation : DbUnitOfWork, TService
    {
        _services.AddScoped<TService>(implementationFactory);
        return this;
    }
}