using System.Data;

namespace Flowsy.Db.Unity;

/// <summary>
/// Describes a database routine (e.g., stored procedure or function).
/// </summary>
public class DbRoutineDescriptor : DbObjectDescriptor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DbRoutineDescriptor"/> class.
    /// </summary>
    /// <param name="fullyQualifiedName">
    /// The fully qualified name of the routine.
    /// </param>
    /// <param name="type">
    /// The type of the routine (stored procedure or function).
    /// </param>
    /// <param name="useNamedParameters">
    /// Indicates whether to use named parameters.
    /// </param>
    /// <param name="returnsTable">
    /// Indicates whether the routine returns a table.
    /// </param>
    /// <param name="parameters">
    /// The parameters of the routine.
    /// </param>
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

    /// <summary>
    /// The type of the routine (stored procedure or function).
    /// </summary>
    public DbRoutineType Type { get; }
    
    /// <summary>
    /// Indicates whether the routine is a stored procedure.
    /// </summary>
    public bool IsProcedure => Type == DbRoutineType.StoredProcedure;
    
    /// <summary>
    /// Indicates whether the routine is a stored function.
    /// </summary>
    public bool IsFunction => Type == DbRoutineType.StoredFunction;
    
    /// <summary>
    /// Indicates whether to use named parameters.
    /// </summary>
    public bool UseNamedParameters { get; }
    
    /// <summary>
    /// Indicates whether the routine returns a table.
    /// </summary>
    public bool ReturnsTable { get; }
    
    /// <summary>
    /// The parameters of the routine.
    /// </summary>
    public IEnumerable<DbParameterDescriptor> Parameters { get; }

    /// <summary>
    /// The command text required to invoke the routine.
    /// </summary>
    public string CommandText { get; }
    
    /// <summary>
    /// The command type required to invoke the routine according to the value of the CommandText property.
    /// </summary>
    public CommandType CommandType { get; }
}