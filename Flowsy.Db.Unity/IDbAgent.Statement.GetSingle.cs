namespace Flowsy.Db.Unity;

public partial interface IDbAgent
{
    T GetSingleFromStatement<T>(string commandText, dynamic? parameters = null);
    
    Task<T> GetSingleFromStatementAsync<T>(string commandText, dynamic? parameters = null, CancellationToken cancellationToken = default);
}