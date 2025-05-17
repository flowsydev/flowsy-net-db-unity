using System.Data;
using Dapper;

namespace Flowsy.Db.Unity;

/// <summary>
/// Represents an event handler that is called after a database command is executed.
/// </summary>
public delegate void DbCommandExecutedEventHandler(object sender, DbCommandExecutedEventArgs e);

/// <summary>
/// Event arguments for the DbCommandExecuted event.
/// </summary>
public class DbCommandExecutedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DbCommandExecutedEventArgs"/> class.
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
    /// <param name="result">
    /// The result of the command execution.
    /// </param>
    public DbCommandExecutedEventArgs(CommandDefinition commandDefinition, IDbConnection connection, IDbTransaction? transaction, object? result)
    {
        CommandDefinition = commandDefinition;
        Connection = connection;
        Transaction = transaction;
        Result = result;
    }

    /// <summary>
    /// Gets the definition of the command that was executed.
    /// </summary>
    public CommandDefinition CommandDefinition { get; }
    
    /// <summary>
    /// Gets the connection that was used to execute the command.
    /// </summary>
    public IDbConnection Connection { get; }
    
    /// <summary>
    /// Gets the transaction that was used to execute the command, if any.
    /// </summary>
    public IDbTransaction? Transaction { get; }
    
    /// <summary>
    /// Gets the result of the command execution.
    /// </summary>
    public object? Result { get; }
}