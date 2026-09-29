using System.Collections.Concurrent;
using System.Data;
using Flowsy.Db.Unity.Conventions;
using Npgsql;
using Npgsql.NameTranslation;

namespace Flowsy.Db.Unity.Postgres;

/// <summary>Creates connections from a reusable PostgreSQL data source per configuration.</summary>
public sealed class PostgresConnectionProvider : IDbConnectionProvider, IDisposable, IAsyncDisposable
{
    private readonly ConcurrentDictionary<DbConnectionConfiguration, Lazy<NpgsqlDataSource>> _dataSources =
        new(ReferenceEqualityComparer.Instance);

    /// <inheritdoc />
    public bool CanHandle(DbConnectionConfiguration configuration)
        => configuration.Provider.Family == DbProviderFamily.Postgres
           && configuration.TryGetProviderConfiguration<DbPostgresConfiguration>(out _);

    /// <inheritdoc />
    public IDbConnection CreateConnection(DbConnectionConfiguration configuration)
    {
        if (!CanHandle(configuration))
            throw new InvalidOperationException(
                $"Connection configuration '{configuration.ConnectionKey}' does not belong to the PostgreSQL extension.");

        var dataSource = _dataSources.GetOrAdd(configuration,
            _ => new Lazy<NpgsqlDataSource>(() => BuildDataSource(configuration), LazyThreadSafetyMode.ExecutionAndPublication));
        return dataSource.Value.CreateConnection();
    }

    private static NpgsqlDataSource BuildDataSource(DbConnectionConfiguration configuration)
    {
        var builder = new NpgsqlDataSourceBuilder(configuration.ConnectionString);
        foreach (var mapping in configuration.Conventions.Enums.Mappings ?? [])
        {
            if (string.IsNullOrWhiteSpace(mapping.DatabaseTypeName))
                continue;
            builder.MapEnum(
                mapping.RuntimeType,
                mapping.DatabaseTypeName,
                new ConventionNameTranslator(mapping, configuration.Conventions));
        }

        configuration.TryGetProviderConfiguration<DbPostgresConfiguration>(out var postgres);
        foreach (var composite in postgres?.CompositeMappings ?? [])
            builder.MapComposite(composite.RuntimeType, composite.DatabaseTypeName, new NpgsqlSnakeCaseNameTranslator());
        return builder.Build();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        foreach (var source in _dataSources.Values.Where(x => x.IsValueCreated))
            source.Value.Dispose();
        _dataSources.Clear();
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        foreach (var source in _dataSources.Values.Where(x => x.IsValueCreated))
            await source.Value.DisposeAsync();
        _dataSources.Clear();
    }

    private sealed class ConventionNameTranslator(DbEnumMapping mapping, DbConventionSet conventions) : INpgsqlNameTranslator
    {
        public string TranslateTypeName(string clrName)
            => mapping.NameTranslator?.TranslateTypeName(mapping.RuntimeType)
               ?? conventions.DefaultCaseStyle.Apply(clrName)
               ?? clrName;

        public string TranslateMemberName(string clrName)
        {
            var enumValue = (Enum)Enum.Parse(mapping.RuntimeType, clrName);
            if (mapping.Values.TryGetValue(enumValue, out var explicitValue))
                return explicitValue;
            return mapping.NameTranslator?.TranslateMemberName(clrName)
                   ?? conventions.DefaultCaseStyle.Apply(clrName)
                   ?? clrName;
        }
    }

}
