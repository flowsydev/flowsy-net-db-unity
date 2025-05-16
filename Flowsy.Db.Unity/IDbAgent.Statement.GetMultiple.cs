using Dapper;

namespace Flowsy.Db.Unity;

public partial interface IDbAgent
{
    /// <summary>
    /// Executes a database statement (SQL command) that returns multiple result sets.
    /// </summary>
    /// <param name="commandText">
    /// The text of the SQL statement to execute.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the statement. This can be an anonymous object or a dictionary.
    /// </param>
    /// <returns>
    /// A <see cref="SqlMapper.GridReader"/> that contains the result sets returned by the statement.
    /// </returns>
    SqlMapper.GridReader GetMultipleFromStatement(string commandText, dynamic? parameters = null);

    /// <summary>
    /// Asynchronously executes a database statement (SQL command) that returns multiple result sets.
    /// </summary>
    /// <param name="commandText">
    /// The text of the SQL statement to execute.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the statement. This can be an anonymous object or a dictionary.
    /// </param>
    /// <param name="cancellationToken">
    /// An optional cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="SqlMapper.GridReader"/> that contains the result sets returned by the statement.
    /// </returns>
    Task<SqlMapper.GridReader> GetMultipleFromStatementAsync(string commandText, dynamic? parameters = null, CancellationToken cancellationToken = default);
}