using Microsoft.Extensions.Configuration;

namespace Flowsy.Db.Unity.Test;

public class Configuration
{
    public static readonly IConfiguration Instance = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build(); 
}