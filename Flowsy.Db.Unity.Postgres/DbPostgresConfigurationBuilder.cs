namespace Flowsy.Db.Unity.Postgres;

/// <summary>Builds PostgreSQL-specific configuration.</summary>
public sealed class DbPostgresConfigurationBuilder
{
    private readonly List<DbPostgresCompositeMapping> _composites = [];

    /// <summary>Registers a composite type on the PostgreSQL data source.</summary>
    public DbPostgresConfigurationBuilder MapComposite<T>(string databaseTypeName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseTypeName);
        _composites.RemoveAll(x => x.RuntimeType == typeof(T));
        _composites.Add(new DbPostgresCompositeMapping(typeof(T), databaseTypeName));
        return this;
    }

    internal DbPostgresConfiguration Build() => new(_composites.ToArray());
}
