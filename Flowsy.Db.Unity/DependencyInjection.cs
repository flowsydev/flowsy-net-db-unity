using Flowsy.Db.Unity.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flowsy.Db.Unity;

public static class DependencyInjection
{
    public static DbUnityServiceBuilder AddDbUnity(this IServiceCollection services, Action<DbUnityOptions> configure)
    {
        var options = new DbUnityOptions(services);
        configure(options);
        options.RegisterServices();

        return new DbUnityServiceBuilder(services);;
    }
}