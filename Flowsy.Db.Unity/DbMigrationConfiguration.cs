namespace Flowsy.Db.Unity;

/// <summary>
/// Represents the configuration for executing database migrations.
/// </summary>
/// <param name="MigrationScriptPath">
/// Path of the directory containing versioned migration scripts.
/// </param>
/// <param name="PreMigrationScriptPath">
/// Optional path of the directory containing scripts to execute before migrations.
/// If <c>null</c>, no pre-migration scripts are executed.
/// </param>
/// <param name="PostMigrationScriptPath">
/// Optional path of the directory containing scripts to execute after migrations.
/// If <c>null</c>, no post-migration scripts are executed.
/// </param>
/// <param name="HistoryTableName">
/// Optional name of the table that stores the history of executed migrations.
/// If <c>null</c>, the default migration system table name is used.
/// </param>
/// <param name="HistorySchemaName">
/// Optional name of the schema where the migration history table is located.
/// If <c>null</c>, the default schema is used.
/// </param>
/// <param name="OutOfOrder">
/// Indicates whether executing migrations out of sequential order is allowed.
/// Default is <c>false</c>, requiring migrations to be executed in order.
/// </param>
public record DbMigrationConfiguration(
    string MigrationScriptPath,
    string? PreMigrationScriptPath = null,
    string? PostMigrationScriptPath = null,
    string? HistoryTableName = null,
    string? HistorySchemaName = null,
    bool OutOfOrder = false
    );