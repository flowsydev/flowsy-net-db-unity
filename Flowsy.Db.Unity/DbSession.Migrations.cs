using System.Data.Common;
using EvolveDb;
using Flowsy.Db.Unity.Resources;
using Microsoft.Extensions.Logging;

namespace Flowsy.Db.Unity;

/// <summary>
/// Contains migration-related functionality for database sessions.
/// </summary>
public partial class DbSession
{
    /// <summary>
    /// Applies database migrations using the configured migration settings.
    /// This method executes pre-migration scripts, applies migrations using EvolveDb, and runs post-migration scripts.
    /// </summary>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous migration operation.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no migration configuration is provided.
    /// </exception>
    /// <exception cref="DirectoryNotFoundException">
    /// Thrown when the migration scripts directory does not exist.
    /// </exception>
    /// <exception cref="FileNotFoundException">
    /// Thrown when pre-migration or post-migration script files do not exist.
    /// </exception>
    public async Task MigrateAsync(CancellationToken cancellationToken = default)
    {
        var operationId = CreateOperationId();

        if (Configuration.Migrations is null)
            throw new InvalidOperationException(Strings.NoMigrationConfigurationProvided);
        
        var (
            migrationScriptPath,
            preMigrationScriptPath,
            postMigrationScriptPath,
            historyTableName,
            historySchemaName,
            outOfOrder
            ) = Configuration.Migrations;
        
        if (!Directory.Exists(migrationScriptPath))
            throw new DirectoryNotFoundException(string.Format(Strings.DirectoryXNotFound, migrationScriptPath));
        
        _logger?.Log(
            Configuration.LogLevel,
            "[ SESSION:{SessionId} > OP:{OperationId} ] Starting migration process{NewLine}{@MigrationConfiguration}",
            SessionId,
            operationId,
            Environment.NewLine,
            Configuration.Migrations
            );

        if (!string.IsNullOrEmpty(preMigrationScriptPath))
        {
            if (!File.Exists(preMigrationScriptPath))
                throw new FileNotFoundException(string.Format(Strings.ScriptXNotFound, preMigrationScriptPath));

            await BeginTransactionAsync(cancellationToken);
            
            try
            {
                await ExecuteScriptAsync(preMigrationScriptPath, cancellationToken);
                await CommitTransactionAsync(cancellationToken);
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
        
        await Task.Run(() =>
        {
            _logger?.Log(
                Configuration.LogLevel,
                "[ SESSION:{SessionId} > OP:{OperationId} ] Applying migrations",
                SessionId,
                operationId
            );
            
            try
            {
                using var connection = Configuration.CreateConnection(true);
                if (connection is not DbConnection dbConnection)
                    throw new InvalidOperationException(string.Format(Strings.ConnectionXIsNotDbConnection, connection.GetType().Name));

                void Log(string m) => _logger?.Log(Configuration.LogLevel, "[ SESSION:{SessionId} > OP:{OperationId} ] {Message}", SessionId, operationId, m);

                var evolve = new Evolve(dbConnection, Log)
                {
                    Locations = [migrationScriptPath],
                    CommandTimeout = Configuration.Conventions.Commands.Timeout,
                    MetadataTableSchema = historySchemaName ?? string.Empty,
                    MetadataTableName = historyTableName ?? "changelog",
                    OutOfOrder = outOfOrder
                };

                evolve.Migrate();
                
                _logger?.LogInformation("[ SESSION:{SessionId} > OP:{OperationId} ] Migration completed successfully", SessionId, operationId);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "[ SESSION:{SessionId} > OP:{OperationId} ] Migration failed", SessionId, operationId);
                throw;
            }
        }, cancellationToken);
        
        if (!string.IsNullOrEmpty(postMigrationScriptPath))
        {
            if (!File.Exists(postMigrationScriptPath))
                throw new FileNotFoundException(string.Format(Strings.ScriptXNotFound, postMigrationScriptPath));

            await BeginTransactionAsync(cancellationToken);
            
            try
            {
                await ExecuteScriptAsync(postMigrationScriptPath, cancellationToken);
                await CommitTransactionAsync(cancellationToken);
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
        
        _logger?.Log(
            Configuration.LogLevel,
            "[ SESSION:{SessionId} > OP:{OperationId} ] Migration process completed",
            SessionId,
            operationId
            );
    }
}
