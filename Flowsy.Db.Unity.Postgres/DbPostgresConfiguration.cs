namespace Flowsy.Db.Unity.Postgres;

/// <summary>Provider-specific configuration used when building a PostgreSQL data source.</summary>
public sealed record DbPostgresConfiguration(
    IReadOnlyCollection<DbPostgresCompositeMapping> CompositeMappings) : IDbProviderConfiguration;

/// <summary>Maps a CLR type to a PostgreSQL composite type.</summary>
public sealed record DbPostgresCompositeMapping(Type RuntimeType, string DatabaseTypeName);
