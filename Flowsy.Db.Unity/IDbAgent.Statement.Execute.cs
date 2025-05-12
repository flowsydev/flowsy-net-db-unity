namespace Flowsy.Db.Unity;

public partial interface IDbAgent
{
    int ExecuteStatement(string commandText, dynamic? parameters = null);
    
    Task<int> ExecuteStatementAsync(string commandText, dynamic? parameters = null, CancellationToken cancellationToken = default);
}