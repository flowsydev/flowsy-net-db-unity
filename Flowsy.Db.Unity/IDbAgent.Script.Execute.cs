namespace Flowsy.Db.Unity;

public partial interface IDbAgent
{
    int ExecuteScript(string scriptPath, dynamic? parameters = null);
    
    Task<int> ExecuteScriptAsync(string scriptPath, CancellationToken cancellationToken = default);
    Task<int> ExecuteScriptAsync(string scriptPath, dynamic? parameters = null, CancellationToken cancellationToken = default);
}