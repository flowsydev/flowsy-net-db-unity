namespace Flowsy.Db.Unity;

public partial interface IDbSession
{
    /// <summary>
    /// Executes database migrations to update the database schema to the latest version.
    /// </summary>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of executing database migrations.
    /// </returns>
    Task MigrateAsync(CancellationToken cancellationToken = default);
}