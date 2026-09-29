using Npgsql;

namespace Flowsy.Db.Unity.Postgres;

/// <summary>Opt-in PostgreSQL configuration extensions.</summary>
public static class PostgresDependencyInjection
{
    /// <summary>Configures a PostgreSQL connection backed by a reusable data source.</summary>
    public static DbConnectionConfigurationBuilder UsePostgres(
        this DbServiceCollectionOptions options,
        string connectionKey,
        string connectionString,
        Action<DbPostgresConfigurationBuilder>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(options);
        options.AddConnectionProvider<PostgresConnectionProvider>();
        return options.UseConnection(connectionKey, connectionString)
            .WithProvider(DbProviderFamily.Postgres, "Npgsql", NpgsqlFactory.Instance)
            .WithPostgres(configure);
    }

    /// <summary>Adds PostgreSQL-specific settings to a previously declared connection.</summary>
    public static DbConnectionConfigurationBuilder WithPostgres(
        this DbConnectionConfigurationBuilder connection,
        Action<DbPostgresConfigurationBuilder>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(connection);
        var builder = new DbPostgresConfigurationBuilder();
        configure?.Invoke(builder);
        return connection.WithProviderConfiguration(builder.Build());
    }
}
