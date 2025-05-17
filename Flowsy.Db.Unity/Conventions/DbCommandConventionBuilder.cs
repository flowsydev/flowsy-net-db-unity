using Dapper;

namespace Flowsy.Db.Unity.Conventions;

/// <summary>
/// Builder for configuring database command conventions.
/// </summary>
public class DbCommandConventionBuilder : DbConventionBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DbCommandConventionBuilder"/> class.
    /// </summary>
    /// <param name="parent">
    /// The parent <see cref="DbConventionSetBuilder"/> instance.
    /// </param>
    internal DbCommandConventionBuilder(DbConventionSetBuilder parent) : base(parent)
    {
    }

    /// <summary>
    /// Sets the command flags for the target <see cref="DbCommandConvention"/> instance.
    /// </summary>
    /// <param name="flags">
    /// The command flags to be set. This can be a combination of <see cref="CommandFlags"/> values.
    /// </param>
    /// <returns>
    /// A reference to the current <see cref="DbCommandConventionBuilder"/> instance, allowing for method chaining.
    /// </returns>
    public DbCommandConventionBuilder UseFlags(CommandFlags? flags)
    {
        Parent.Conventions.Commands.Flags = flags;
        return this;
    }
    
    /// <summary>
    /// Sets the timeout for the target <see cref="DbCommandConvention"/> instance.
    /// </summary>
    /// <param name="timeout">
    /// The timeout value in seconds. This can be null, in which case the default timeout will be used.
    /// </param>
    /// <returns>
    /// A reference to the current <see cref="DbCommandConventionBuilder"/> instance, allowing for method chaining.
    /// </returns>
    public DbCommandConventionBuilder UseTimeout(int? timeout)
    {
        Parent.Conventions.Commands.Timeout = timeout;
        return this;
    }
}