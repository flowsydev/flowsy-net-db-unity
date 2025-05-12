using System.Data;
using Dapper;

namespace Flowsy.Db.Unity;

public class DbCommandExecutedEventArgs
{
    public DbCommandExecutedEventArgs(CommandDefinition commandDefinition, IDbConnection connection, IDbTransaction? transaction, object? result)
    {
        CommandDefinition = commandDefinition;
        Connection = connection;
        Transaction = transaction;
        Result = result;
    }

    public CommandDefinition CommandDefinition { get; }
    public IDbConnection Connection { get; }
    public IDbTransaction? Transaction { get; }
    public object? Result { get; }
}