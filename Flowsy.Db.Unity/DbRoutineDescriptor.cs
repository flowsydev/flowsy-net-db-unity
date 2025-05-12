using System.Data;

namespace Flowsy.Db.Unity;

/// <summary>
/// Describes a database routine (e.g., stored procedure or function).
/// </summary>
public class DbRoutineDescriptor : DbObjectDescriptor
{
    public DbRoutineDescriptor(DbFullyQualifiedName fullyQualifiedName, DbRoutineType type, bool useNamedParameters = false, bool returnsTable = false, IEnumerable<DbParameterDescriptor>? parameters = null) : base(fullyQualifiedName)
    {
        Type = type;
        UseNamedParameters = useNamedParameters;
        ReturnsTable = returnsTable;
        Parameters = parameters ?? [];
        CommandText = type == DbRoutineType.StoredProcedure
            ? fullyQualifiedName.ToString()
            : Provider.FormatRoutineCall(this);
        CommandType = type == DbRoutineType.StoredProcedure
            ? CommandType.StoredProcedure
            : CommandType.Text;
    }

    public DbRoutineType Type { get; }
    
    public bool IsProcedure => Type == DbRoutineType.StoredProcedure;
    public bool IsFunction => Type == DbRoutineType.StoredFunction;
    public bool UseNamedParameters { get; }
    public bool ReturnsTable { get; }
    
    public IEnumerable<DbParameterDescriptor> Parameters { get; }

    public string CommandText { get; }
    
    public CommandType CommandType { get; }
}