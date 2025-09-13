using Dapper;

namespace Flowsy.Db.Unity.Conventions;

/// <summary>
/// Represents conventions for database commands.
/// </summary>
/// <param name="Timeout">The command timeout in seconds. If null, uses the default timeout.</param>
/// <param name="Flags">The command flags to use for query execution.</param>
public record DbCommandConvention(int? Timeout = null, CommandFlags Flags = CommandFlags.Buffered) : DbConvention
{
    /// <summary>
    /// Default command convention with no timeout and buffered command flags.
    /// </summary>
    public static readonly DbCommandConvention Default = new();
}