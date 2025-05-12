using Dapper;

namespace Flowsy.Db.Unity.Conventions;

public class DbCommandConventionBuilder : DbConventionBuilder
{
    public DbCommandConventionBuilder(DbConventionSetBuilder parent) : base(parent)
    {
    }

    public DbCommandConventionBuilder UseFlags(CommandFlags? flags)
    {
        Parent.Conventions.Commands.Flags = flags;
        return this;
    }
    
    public DbCommandConventionBuilder UseTimeout(int? timeout)
    {
        Parent.Conventions.Commands.Timeout = timeout;
        return this;
    }
}