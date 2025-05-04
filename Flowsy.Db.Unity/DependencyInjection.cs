using Flowsy.Db.Unity.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flowsy.Db.Unity;

public static class DependencyInjection
{
    public static DbUnityServiceBuilder AddDbUnity(this IServiceCollection services, Action<DbUnityOptions> configure)
    {
        var serviceBuilder = new DbUnityServiceBuilder(services);
        
        var options = new DbUnityOptions(services, serviceBuilder);
        configure(options);
        options.RegisterServices();

        return serviceBuilder;
    }
}