using Dapper;

namespace Flowsy.Db.Unity;

public partial interface IDbAgent
{
    SqlMapper.GridReader GetMultipleFromStatement(string commandText, dynamic? parameters = null);
    
    Task<SqlMapper.GridReader> GetMultipleFromStatementAsync(string commandText, dynamic? parameters = null, CancellationToken cancellationToken = default);
}