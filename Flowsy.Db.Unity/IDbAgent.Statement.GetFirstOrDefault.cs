namespace Flowsy.Db.Unity;

public partial interface IDbAgent
{
    T? GetFirstOrDefaultFromStatement<T>(string commandText, dynamic? parameters = null);
    
    Task<T?> GetFirstOrDefaultFromStatementAsync<T>(string commandText, dynamic? parameters = null, CancellationToken cancellationToken = default);
}