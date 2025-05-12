namespace Flowsy.Db.Unity;

public partial interface IDbAgent
{
    T? GetSingleOrDefaultFromStatement<T>(string commandText, dynamic? parameters = null);
    
    Task<T?> GetSingleOrDefaultFromStatementAsync<T>(string commandText, dynamic? parameters = null, CancellationToken cancellationToken = default);
}