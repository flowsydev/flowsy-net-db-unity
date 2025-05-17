using System.Data;
using Dapper;

namespace Flowsy.Db.Unity;

/// <summary>
/// Represents an event handler that is called before a database command is executed.
/// </summary>
public delegate void DbCommandExecutingEventHandler(object sender, DbCommandExecutingEventArgs e);

/// <summary>
/// Event arguments for the DbCommandExecuted event.
/// </summary>
public class DbCommandExecutingEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DbCommandExecutingEventArgs"/> class.
    /// </summary>
    /// <param name="commandDefinition">
    /// The command definition.
    /// </param>
    /// <param name="connection">
    /// The database connection.
    /// </param>
    /// <param name="transaction">
    /// The database transaction, if any.
    /// </param>
    public DbCommandExecutingEventArgs(CommandDefinition commandDefinition, IDbConnection connection, IDbTransaction? transaction)
    {
        CommandDefinition = commandDefinition;
        Connection = connection;
        Transaction = transaction;
    }

    /// <summary>
    /// Gets the definition of the command that is being executed.
    /// </summary>
    public CommandDefinition CommandDefinition { get; }
    
    /// <summary>
    /// Gets the database connection that is being used to execute the command.
    /// </summary>
    public IDbConnection Connection { get; }
    
    /// <summary>
    /// Gets the database transaction that is being used to execute the command, if any.
    /// </summary>
    public IDbTransaction? Transaction { get; }
}