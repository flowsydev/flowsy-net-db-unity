using Dapper;

namespace Flowsy.Db.Unity;

public partial interface IDbSession
{
    /// <summary>
    /// Performs a database query that can return multiple result sets.
    /// </summary>
    /// <param name="statement">
    /// SQL statement to execute that may contain multiple queries.
    /// </param>
    /// <param name="parameters">
    /// Optional parameters for the query, which can be used to avoid SQL injections.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous database query operation.
    /// The result is a GridReader object that allows reading multiple result sets.
    /// </returns>
    Task<SqlMapper.GridReader> QueryMultipleAsync(
        string statement,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default
    );
}
