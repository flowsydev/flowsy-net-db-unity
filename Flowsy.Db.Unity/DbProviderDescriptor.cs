using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using Flowsy.Db.Unity.Resources;

namespace Flowsy.Db.Unity;

/// <summary>
/// Represents a database provider descriptor.
/// </summary>
public class DbProviderDescriptor
{
    public static readonly DbProviderDescriptor Generic = new (DbProviderFamily.Generic);
    private static readonly ConcurrentDictionary<DbProviderFamily, ConcurrentDictionary<Type, DbType>> TypeMappings = [];
    
    /// <summary>
    /// Initializes a new instance of the <see cref="DbProviderDescriptor"/> class.
    /// </summary>
    /// <param name="family">
    /// The family of the database provider (e.g., Postgres, MySql, etc.).
    /// </param>
    /// <param name="invariantName">
    /// The invariant name of the implementation of the database provider.
    /// </param>
    /// <param name="factory">
    /// The factory for creating database connections.
    /// </param>
    public DbProviderDescriptor(DbProviderFamily family, string? invariantName = null, DbProviderFactory? factory = null)
    {
        Family = family;
        InvariantName = invariantName;
        Factory = factory;
        ParameterPrefixForStatement = "@";
        SupportsSchemas = false;
        SupportsNamedParameters = false;
        SupportsEnums = false;
        SupportsEnumsAsCustomTypes = false;
        SupportsArrays = false;
        switch (Family)
        {
            case DbProviderFamily.Postgres:
                DefaultPort = 5432;
                DefaultDatabaseName = "postgres";
                DefaultSchemaName = "public";
                SupportsSchemas = true;
                SupportsNamedParameters = true;
                SupportsEnums = true;
                SupportsEnumsAsCustomTypes = true;
                SupportsArrays = true;
                break;
            case DbProviderFamily.MySql:
                DefaultPort = 3306;
                DefaultDatabaseName = "mysql";
                DefaultSchemaName = null;
                SupportsEnums = true;
                break;
            case DbProviderFamily.SqlServer:
                DefaultPort = 1433;
                DefaultDatabaseName = "master";
                DefaultSchemaName = "dbo";
                SupportsSchemas = true;
                SupportsNamedParameters = true;
                break;
            case DbProviderFamily.Oracle:
                DefaultPort = 1521;
                DefaultDatabaseName = null;
                DefaultSchemaName = null;
                ParameterPrefixForStatement = ":";
                SupportsNamedParameters = true;
                break;
            case DbProviderFamily.Sqlite:
                DefaultPort = 0;
                DefaultDatabaseName = null;
                DefaultSchemaName = null;
                break;
            default:
                DefaultPort = 0;
                DefaultDatabaseName = null;
                DefaultSchemaName = null;
                break;
        }
    }

    /// <summary>
    /// The family of the database provider (e.g., Postgres, MySql, etc.).
    /// </summary>
    public DbProviderFamily Family { get; }
    
    /// <summary>
    /// The invariant name of the implementation of the database provider.
    /// </summary>
    public string? InvariantName { get; }
    
    /// <summary>
    /// The factory for creating database connections.
    /// </summary>
    public DbProviderFactory? Factory { get; }

    /// <summary>
    /// The value used by the database provider to separate objects in a fully qualified name.
    /// </summary>
    /// <exception cref="NotSupportedException">
    /// Thrown when the provider family is not supported.
    /// </exception>
    public string ObjectSeparator => Family switch
    {
        DbProviderFamily.Generic => ".",
        DbProviderFamily.Postgres => ".",
        DbProviderFamily.MySql => ".",
        DbProviderFamily.SqlServer => ".",
        DbProviderFamily.Oracle => ".",
        DbProviderFamily.Sqlite => ".",
        _ => throw new NotSupportedException(string.Format(Strings.ProviderXNotSupported, Family.ToString()))
    };
    
    /// <summary>
    /// The default port for the provider.
    /// </summary>
    public int DefaultPort { get; }
    
    /// <summary>
    /// The default database name for the provider.
    /// </summary>
    public string? DefaultDatabaseName { get; }
    
    /// <summary>
    /// The default schema for the provider.
    /// </summary>
    public string? DefaultSchemaName { get; }
    
    /// <summary>
    /// The parameter prefix for a statement.
    /// </summary>
    public string ParameterPrefixForStatement { get; }
    
    /// <summary>
    /// A value indicating whether the provider supports schemas.
    /// </summary>
    public bool SupportsSchemas { get; }
    
    /// <summary>
    /// A value indicating whether the provider supports named parameters.
    /// </summary>
    public bool SupportsNamedParameters { get; }
    
    /// <summary>
    /// A value indicating whether the provider supports enums.
    /// </summary>
    public bool SupportsEnums { get; }
    
    /// <summary>
    /// A value indicating whether the provider supports enums as custom types.
    /// </summary>
    public bool SupportsEnumsAsCustomTypes { get; }
    
    /// <summary>
    /// A value indicating whether the provider supports arrays.
    /// </summary>
    public bool SupportsArrays { get; }
    
    /// <summary>
    /// Gets a value indicating whether the provider supports the specified routine type.
    /// </summary>
    /// <param name="routineType">
    /// The type of the routine (e.g., stored procedure, stored function).
    /// </param>
    /// <returns>
    /// True if the provider supports the specified routine type; otherwise, false.
    /// </returns>
    public bool SupportsRoutineType(DbRoutineType routineType)
        => Family != DbProviderFamily.Sqlite || routineType != DbRoutineType.StoredProcedure;
    
    /// <summary>
    /// Gets a value indicating whether the provider can return a table from the specified routine type.
    /// </summary>
    /// <param name="routineType">
    /// The type of the routine (e.g., stored procedure, stored function).
    /// </param>
    /// <returns>
    /// True if the provider can return a table from the specified routine type; otherwise, false.
    /// </returns>
    public bool RoutineCanReturnTable(DbRoutineType routineType)
        => routineType == DbRoutineType.StoredProcedure || Family switch
        {
            DbProviderFamily.Postgres => true,
            DbProviderFamily.MySql => false,
            DbProviderFamily.SqlServer => true,
            DbProviderFamily.Oracle => true,
            DbProviderFamily.Sqlite => false,
            _ => false
        };
    
    /// <summary>
    /// Parses an object name into a fully qualified name.
    /// </summary>
    /// <param name="name">
    /// The object name to parse.
    /// </param>
    /// <param name="transform">
    /// An optional function to transform each part of the name during parsing.
    /// </param>
    /// <returns>
    /// A <see cref="DbFullyQualifiedName"/> representing the parsed object name.
    /// </returns>
    public DbFullyQualifiedName ParseObjectName(string name, Func<string, int, int, string>? transform = null)
    {
        var parts = name.Split(ObjectSeparator, StringSplitOptions.RemoveEmptyEntries);

        if (transform is null)
            return new DbFullyQualifiedName(this, parts);
        
        var index = 0;
        var length = parts.Length;
        foreach (var part in parts)
        {
            parts[index] = transform(part, index, length);
            index++;
        }

        return new DbFullyQualifiedName(this, parts);
    }
    
    /// <summary>
    /// Casts the given expression to the specified type using the appropriate syntax for the provider.
    /// </summary>
    /// <param name="expression">
    /// The expression to cast.
    /// </param>
    /// <param name="type">
    /// The type to cast to.
    /// </param>
    /// <returns>
    /// The formatted casting expression.
    /// </returns>
    public string FormatCasting(string expression, string type)
        => Family switch
        {
            DbProviderFamily.Postgres => $"{expression}::{type}",
            _ => $"CAST({expression} AS {type})"
        };
    
    /// <summary>
    /// Builds an expression for a named parameter using the appropriate syntax for the provider.
    /// </summary>
    /// <param name="parameterName">
    /// The name of the parameter.
    /// </param>
    /// <param name="valueExpression">
    /// The expression for the value of the parameter.
    /// </param>
    /// <returns>
    /// The formatted named parameter expression.
    /// </returns>
    public string FormatNamedParameter(string parameterName, string valueExpression)
        => Family switch
        {
            DbProviderFamily.Postgres => $"{parameterName} => {valueExpression}",
            DbProviderFamily.SqlServer => $"{parameterName} = {valueExpression}",
            _ => $"{ParameterPrefixForStatement}{parameterName}",
        };
    
    /// <summary>
    /// Gets the database type for the specified runtime type.
    /// </summary>
    /// <param name="runtimeType">
    /// The runtime type to get the database type for.
    /// </param>
    /// <returns>
    /// The database type corresponding to the runtime type, or null if cannot be determined.
    /// </returns>
    public DbType? GetDatabaseType(Type runtimeType)
    {
        var mappings = TypeMappings.GetOrAdd(Family, CreateDefaultTypeMappings);
        return mappings?.TryGetValue(runtimeType, out var dbType) ?? false ? dbType : null;
    }

    /// <summary>
    /// Builds the expression for an array type using the appropriate syntax for the provider.
    /// </summary>
    /// <param name="databaseCustomType">
    /// The database custom type to format as an array.
    /// </param>
    /// <returns>
    /// The formatted array type expression, or null if the provider does not support arrays.
    /// </returns>
    public string? FormatArrayType(string? databaseCustomType)
        => Family == DbProviderFamily.Postgres ? $"{databaseCustomType}[]" : null;

    /// <summary>
    /// Builds the SQL statement for calling the specified routine using the appropriate syntax for the provider.
    /// </summary>
    /// <param name="routine">
    /// The routine descriptor containing information about the routine.
    /// </param>
    /// <returns>
    /// The formatted SQL statement for calling the routine.
    /// </returns>
    public string FormatRoutineCall(DbRoutineDescriptor routine)
        => FormatRoutineCall(routine.FullyQualifiedName.ToString(), routine.Type, routine.UseNamedParameters, routine.ReturnsTable, routine.Parameters.ToArray());
    
    /// <summary>
    /// Builds the SQL statement for calling the specified routine using the appropriate syntax for the provider.
    /// </summary>
    /// <param name="fullyQualifiedName">
    /// The fully qualified name of the routine.
    /// </param>
    /// <param name="routineType">
    /// The type of the routine (e.g., stored procedure, stored function).
    /// </param>
    /// <param name="useNamedParameters">
    /// A value indicating whether to use named parameters in the call.
    /// </param>
    /// <param name="returnsTable">
    /// A value indicating whether the routine returns a table.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the routine.
    /// </param>
    /// <returns>
    /// The formatted SQL statement for calling the routine.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// Thrown when the provider family does not support the specified routine type or when the routine type cannot return a table.
    /// </exception>
    public string FormatRoutineCall(string fullyQualifiedName, DbRoutineType routineType, bool useNamedParameters = false, bool returnsTable = false, params DbParameterDescriptor[] parameters)
    {
        if (returnsTable && !RoutineCanReturnTable(routineType))
            throw new NotSupportedException(string.Format(Strings.ProviderXCanNotReturnATableFromRoutineOfTypeY, Family, routineType));
        
        List<string> parameterNames = [];
        List<string> parameterExpressions = [];
        foreach (var parameter in parameters)
        {
            parameterNames.Add(parameter.Name);
            
            var expression = $"{ParameterPrefixForStatement}{parameter.Name}";
            if (parameter is {ValueExpression: DbValueExpression.CustomTypeCast, CustomType: not null})
                expression = FormatCasting(expression, parameter.CustomType);
            
            parameterExpressions.Add(expression);
        }
        var parameterListText = useNamedParameters 
            ? string.Join(", ", parameterExpressions.Select((e, index) => FormatNamedParameter(parameterNames[index], e)))
            : string.Join(", ", parameterExpressions);
        
        var unsupportedRoutineTypeMessage = string.Format(Strings.ProviderXDoesNotSupportRoutineTypeY, Family, routineType);

        return Family switch
        {
            DbProviderFamily.Postgres => routineType switch
            {
                DbRoutineType.StoredProcedure => $"CALL {fullyQualifiedName}({parameterListText})",
                DbRoutineType.StoredFunction => returnsTable
                    ? $"SELECT * FROM {fullyQualifiedName}({parameterListText})"
                    : $"SELECT {fullyQualifiedName}({parameterListText})",
                _ => throw new NotSupportedException(unsupportedRoutineTypeMessage)
            },
            DbProviderFamily.MySql => routineType switch
            {
                DbRoutineType.StoredProcedure => $"CALL {fullyQualifiedName}({parameterListText})",
                DbRoutineType.StoredFunction => returnsTable
                    ? throw new NotSupportedException(unsupportedRoutineTypeMessage)
                    : $"SELECT {fullyQualifiedName}({parameterListText})",
                _ => throw new NotSupportedException(unsupportedRoutineTypeMessage)
            },
            DbProviderFamily.SqlServer => routineType switch
            {
                DbRoutineType.StoredProcedure => $"EXEC {fullyQualifiedName} {parameterListText}",
                DbRoutineType.StoredFunction => returnsTable
                    ? $"SELECT * FROM {fullyQualifiedName}({parameterListText})"
                    : $"SELECT {fullyQualifiedName}({parameterListText})",
                _ => throw new NotSupportedException(unsupportedRoutineTypeMessage)
            },
            DbProviderFamily.Oracle => routineType switch
            {
                DbRoutineType.StoredProcedure => $"BEGIN {fullyQualifiedName}({parameterListText}); END;",
                DbRoutineType.StoredFunction => returnsTable
                    ? $"SELECT * FROM TABLE({fullyQualifiedName}({parameterListText}))"
                    : $"SELECT {fullyQualifiedName}({parameterListText}) FROM DUAL",
                _ => throw new NotSupportedException(unsupportedRoutineTypeMessage)
            },
            DbProviderFamily.Sqlite => routineType switch
            {
                DbRoutineType.StoredProcedure => throw new NotSupportedException(unsupportedRoutineTypeMessage),
                DbRoutineType.StoredFunction => returnsTable
                    ? throw new NotSupportedException(unsupportedRoutineTypeMessage)
                    : $"SELECT {fullyQualifiedName}({parameterListText})",
                _ => throw new NotSupportedException(unsupportedRoutineTypeMessage)
            },
            _ => throw new NotSupportedException(unsupportedRoutineTypeMessage)
        };
    }
    
    private static ConcurrentDictionary<Type, DbType> CreateDefaultTypeMappings(DbProviderFamily provider)
    {
        var map = new ConcurrentDictionary<Type, DbType>
        {
            [typeof(byte)] = DbType.Byte,
            [typeof(sbyte)] = DbType.SByte,
            [typeof(short)] = DbType.Int16,
            [typeof(ushort)] = DbType.UInt16,
            [typeof(int)] = DbType.Int32,
            [typeof(uint)] = DbType.UInt32,
            [typeof(long)] = DbType.Int64,
            [typeof(ulong)] = DbType.UInt64,
            [typeof(float)] = DbType.Single,
            [typeof(double)] = DbType.Double,
            [typeof(decimal)] = DbType.Decimal,
            [typeof(bool)] = DbType.Boolean,
            [typeof(string)] = DbType.String,
            [typeof(char)] = DbType.StringFixedLength,
            [typeof(Guid)] = DbType.Guid,
            [typeof(DateTime)] = DbType.DateTime,
            [typeof(DateTimeOffset)] = DbType.DateTimeOffset,
            [typeof(byte[])] = DbType.Binary
        };

        // Add provider-specific overrides here if needed
        switch (provider)
        {
            case DbProviderFamily.Postgres:
                map[typeof(DateTime)] = DbType.DateTime2;
                break;
            
            case DbProviderFamily.Sqlite:
                // SQLite treats everything as Text/Integer/Real/Blob
                map[typeof(bool)] = DbType.Int32;
                map[typeof(DateTime)] = DbType.String;
                map[typeof(DateTimeOffset)] = DbType.String;
                break;
        }

        return map;
    }
}