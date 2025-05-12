using System.Data;
using Dapper;

namespace Flowsy.Db.Unity.Conventions;

public class DbCommandConvention : DbConvention
{
    internal DbCommandConvention(DbConventionSet conventions) : base(conventions)
    {
    }
    
    public CommandFlags? Flags { get; internal set; }
    public int? Timeout { get; internal set; }

    public CommandDefinition BuildDefinition(string commandText, dynamic? parameters = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
        => new (
            commandText,
            Conventions.Parameters.BuildDynamicParameters(parameters as object),
            transaction,
            Timeout,
            CommandType.Text,
            Flags ?? CommandFlags.Buffered,
            cancellationToken
            );

    public CommandDefinition BuildDefinition(DbRoutineDescriptor routineDescriptor, dynamic? parameters = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var dynamicParameters = Conventions.Parameters.BuildDynamicParameters(routineDescriptor.Parameters, parameters as object);
        return new CommandDefinition(
            routineDescriptor.CommandText,
            dynamicParameters,
            transaction,
            Timeout,
            routineDescriptor.CommandType,
            Flags ?? CommandFlags.Buffered,
            cancellationToken
            );
    }
    
    public void CopyTo(DbCommandConvention other)
    {
        other.Flags = Flags;
        other.Timeout = Timeout;
    }
    
    public DbCommandConvention Clone(DbConventionSet conventions)
    {
        var clone = new DbCommandConvention(conventions);
        CopyTo(clone);
        return clone;
    }
}