using System.Data.Common;
using Flowsy.Db.Unity.Conventions;

namespace Flowsy.Db.Unity.Configuration;

public class DbConnectionOptionsBuilder
{
    private readonly DbConnectionOptions _options;
    private readonly DbConventionSetBuilder _conventionSetBuilder;

    public DbConnectionOptionsBuilder(DbConnectionOptions options)
    {
        _options = options;
        _conventionSetBuilder = new DbConventionSetBuilder(options.Provider);
    }

    public DbConnectionOptionsBuilder AsDefault(bool @default = true)
    {
        _options.Default = @default;
        return this;
    }

    public DbConnectionOptionsBuilder WithProvider(DbProviderFamily family, string invariantName, DbProviderFactory factory)
    {
        DbProviderFactories.RegisterFactory(invariantName, factory);
        _options.Provider = new DbProvider(family, invariantName, factory);
        return this;
    }
    
    public DbConnectionOptionsBuilder WithConnectionString(string connectionString)
    {
        _options.ConnectionString = connectionString;
        return this;
    }
    
    public DbConventionSetBuilder WithConventions() => _conventionSetBuilder;

    internal DbConnectionOptions Build()
    {
        _options.Conventions = _conventionSetBuilder.Build();
        return _options;
    }
}