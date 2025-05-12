using System.Data;
using Dapper;

namespace Flowsy.Db.Unity;

public class DbCommandExecutingEventArgs
{
    public DbCommandExecutingEventArgs(CommandDefinition commandDefinition, IDbConnection connection, IDbTransaction? transaction)
    {
        CommandDefinition = commandDefinition;
        Connection = connection;
        Transaction = transaction;
    }

    public CommandDefinition CommandDefinition { get; }
    public IDbConnection Connection { get; }
    public IDbTransaction? Transaction { get; }
}