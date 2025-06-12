using Flowsy.Db.Unity.Configuration;
using Flowsy.Db.Unity.Resources;
using Microsoft.Extensions.DependencyInjection;

namespace Flowsy.Db.Unity;

public static class DependencyInjection
{
    /// <summary>
    /// Adds a set of services to the <see cref="IServiceCollection"/> to unify database operations.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add the services to.
    /// </param>
    /// <param name="configure">
    /// An action to configure the connection options and conventions for database operations.
    /// </param>
    /// <returns>
    /// A <see cref="DbUnityServiceBuilder"/> that can be used to configure the services further.
    /// </returns>
    public static DbUnityServiceBuilder AddDbUnity(this IServiceCollection services, Action<DbUnityOptions> configure)
    {
        var options = new DbUnityOptions();
        configure(options);
        
        var connectionOptionsList = options.BuildConnectionOptions().ToList();
        if (!connectionOptionsList.Any())
            throw new InvalidOperationException(Strings.NoConnectionOptionsDefined);
        
        var distinctKeys = connectionOptionsList.Select(o => o.ConnectionKey).Distinct();
        if (distinctKeys.Count() != connectionOptionsList.Count)
            throw new InvalidOperationException(Strings.ConnectionKeyMustBeUnique);
        
        var defaultConnectionOptionsList = connectionOptionsList.Where(o => o.Default).ToList();
        if (defaultConnectionOptionsList.Count > 1)
            throw new InvalidOperationException(Strings.OnlyOneDefaultConnectionAllowed);
        
        var defaultConnectionOptions = defaultConnectionOptionsList.FirstOrDefault();
        defaultConnectionOptions ??= connectionOptionsList.First();

        // Register the default (unnamed) connection options
        services
            .AddOptions<DbConnectionOptions>()
            .Configure(o => defaultConnectionOptions.CopyTo(o));
        
        // Register the named connection options
        foreach (var connectionOptions in connectionOptionsList)
        {
            services
                .AddOptions<DbConnectionOptions>(connectionOptions.ConnectionKey)
                .Configure(o => connectionOptions.CopyTo(o));
        }
         
        return new DbUnityServiceBuilder(services);;
    }
}