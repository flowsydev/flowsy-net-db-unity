using System.Data.Common;
using Flowsy.Db.Unity.Conventions;
using Flowsy.Db.Unity.Resources;
using Microsoft.Extensions.Logging;

namespace Flowsy.Db.Unity;

public class DbConnectionOptionsBuilder
{
    private readonly DbConnectionOptions _options;
    private DbConventionSetBuilder _conventionSetBuilder;
    private bool _providerSet;
    
    public DbConnectionOptionsBuilder(string connectionKey) : this(new DbConnectionOptions(connectionKey))
    {
    }

    public DbConnectionOptionsBuilder(DbConnectionOptions options)
    {
        _options = options;
        _conventionSetBuilder = new DbConventionSetBuilder(options.Provider);
        _providerSet = true;
    }

    public DbConnectionOptionsBuilder AsDefault(bool @default = true)
    {
        _options.Default = @default;
        return this;
    }

    public DbConnectionOptionsBuilder WithProvider(DbProviderDescriptor provider)
    {
        _options.Provider = provider;
        _conventionSetBuilder = new DbConventionSetBuilder(provider);
        _providerSet = true;
        return this;
    }

    public DbConnectionOptionsBuilder WithProvider(DbProviderFamily family, string invariantName, DbProviderFactory factory)
    {
        _options.Provider = new DbProviderDescriptor(family, invariantName, factory);
        _conventionSetBuilder = new DbConventionSetBuilder(_options.Provider);
        _providerSet = true;
        return this;
    }
    
    public DbConnectionOptionsBuilder WithConnectionString(string connectionString)
    {
        _options.ConnectionString = connectionString;
        return this;
    }
    
    public DbConnectionOptionsBuilder WithLogLevel(LogLevel logLevel)
    {
        _options.LogLevel = logLevel;
        return this;
    }

    public DbConnectionOptionsBuilder WithConventions(DbConventionSet conventions)
    {
        _conventionSetBuilder = new DbConventionSetBuilder(conventions);
        _options.Provider = _conventionSetBuilder.Conventions.Provider;
        _providerSet = true;
        return this;
    }

    public DbConventionSetBuilder WithConventions()
    {
        if (!_providerSet)
            throw new InvalidOperationException(Strings.ProviderMustBeSetBeforeConfiguringConventions);
        
        return _conventionSetBuilder;
    }

    public DbConnectionOptions Build()
    {
        _options.Conventions = _conventionSetBuilder.Build();
        return _options;
    }
}