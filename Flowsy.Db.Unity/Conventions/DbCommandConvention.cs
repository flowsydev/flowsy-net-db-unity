using System.Data;
using Dapper;

namespace Flowsy.Db.Unity.Conventions;

/// <summary>
/// Represents a convention for executing database commands.
/// </summary>
public class DbCommandConvention : DbConvention
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DbCommandConvention"/> class.
    /// </summary>
    /// <param name="conventions">
    /// The <see cref="DbConventionSet"/> to which this convention belongs.
    /// </param>
    internal DbCommandConvention(DbConventionSet conventions) : base(conventions)
    {
    }
    
    /// <summary>
    /// The flags that control the behavior of the command execution.
    /// </summary>
    public CommandFlags? Flags { get; internal set; }
    
    /// <summary>
    /// The timeout value for the command execution in seconds.
    /// </summary>
    public int? Timeout { get; internal set; }

    /// <summary>
    /// Builds a <see cref="CommandDefinition"/> for executing a command with the specified parameters and transaction.
    /// </summary>
    /// <param name="commandText">
    /// The SQL command text to be executed.
    /// </param>
    /// <param name="parameters">
    /// The parameters to be passed to the command. This can be an anonymous object or a dictionary.
    /// </param>
    /// <param name="transaction">
    /// An optional transaction to be used for the command execution.
    /// </param>
    /// <param name="cancellationToken">
    /// An optional cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="CommandDefinition"/> instance that encapsulates the command text, parameters, transaction, timeout, command type, flags, and cancellation token.
    /// </returns>
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

    /// <summary>
    /// Builds a <see cref="CommandDefinition"/> for executing a database routine with the specified parameters and transaction.
    /// </summary>
    /// <param name="routineDescriptor">
    /// The descriptor of the database routine to be executed.
    /// </param>
    /// <param name="parameters">
    /// The parameters to be passed to the routine. This can be an anonymous object or a dictionary.
    /// </param>
    /// <param name="transaction">
    /// An optional transaction to be used for the command execution
    /// </param>
    /// <param name="cancellationToken">
    /// An optional cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="CommandDefinition"/> instance that encapsulates the command text, parameters, transaction, timeout, command type, flags, and cancellation token.
    /// </returns>
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
    
    /// <summary>
    /// Copies the properties of this <see cref="DbCommandConvention"/> to another instance.
    /// </summary>
    /// <param name="other">
    /// The <see cref="DbCommandConvention"/> instance to which the properties will be copied.
    /// </param>
    public void CopyTo(DbCommandConvention other)
    {
        other.Flags = Flags;
        other.Timeout = Timeout;
    }
    
    /// <summary>
    /// Creates a clone of this <see cref="DbCommandConvention"/> instance.
    /// </summary>
    /// <param name="conventions">
    /// The <see cref="DbConventionSet"/> to which the cloned convention will belong.
    /// </param>
    /// <returns>
    /// A new instance of <see cref="DbCommandConvention"/> with the same properties as this instance.
    /// </returns>
    public DbCommandConvention Clone(DbConventionSet conventions)
    {
        var clone = new DbCommandConvention(conventions);
        CopyTo(clone);
        return clone;
    }
}