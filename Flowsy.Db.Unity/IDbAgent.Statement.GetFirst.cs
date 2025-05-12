namespace Flowsy.Db.Unity;

public partial interface IDbAgent
{
    T GetFirstFromStatement<T>(string commandText, dynamic? parameters = null);
    
    Task<T> GetFirstFromStatementAsync<T>(string commandText, dynamic? parameters = null, CancellationToken cancellationToken = default);
}