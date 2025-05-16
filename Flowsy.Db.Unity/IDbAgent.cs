using System.Data;

namespace Flowsy.Db.Unity;

/// <summary>
/// Represents a database agent that performs operations on a database.
/// </summary>
public partial interface IDbAgent : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// The database connection associated with this agent.
    /// </summary>
    IDbConnection Connection { get; }
    
    /// <summary>
    /// An optional unit of work associated with this agent.
    /// A unit of work is a pattern that allows you to group multiple database operations into a single transaction.
    /// </summary>
    IDbUnitOfWork? UnitOfWork { get; }
    
    /// <summary>
    /// Raised when a command is about to be executed.
    /// </summary>
    event DbCommandExecutingEventHandler? CommandExecuting;
    
    /// <summary>
    /// Raised when a command has been executed.
    /// </summary>
    event DbCommandExecutedEventHandler? CommandExecuted;
}