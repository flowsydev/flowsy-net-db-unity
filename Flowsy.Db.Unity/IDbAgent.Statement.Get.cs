namespace Flowsy.Db.Unity;

public partial interface IDbAgent
{
    IEnumerable<T> GetFromStatement<T>(string commandText, dynamic? parameters = null);
    
    Task<IEnumerable<T>> GetFromStatementAsync<T>(string commandText, dynamic? parameters = null, CancellationToken cancellationToken = default);
}