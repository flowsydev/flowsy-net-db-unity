using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using Flowsy.Db.Unity.Resources;

namespace Flowsy.Db.Unity;

/// <summary>
/// Encapsulates the information of a database provider.
/// </summary>
public class DbProviderDescriptor
{
    /// <summary>
    /// Gets the provider-neutral descriptor used when no database-specific provider is configured.
    /// </summary>
    public static readonly DbProviderDescriptor Generic = new (DbProviderFamily.Generic);
    private static readonly ConcurrentDictionary<DbProviderFamily, ConcurrentDictionary<Type, DbType>> TypeMappings = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="DbProviderDescriptor"/> class.
    /// </summary>
    /// <param name="family">
    /// Database provider family.
    /// </param>
    /// <param name="invariantName">
    /// Implementation name of the data provider used for communication with the database engine.
    /// </param>
    /// <param name="factory">
    /// Data provider factory used to create connections, commands, etc.
    /// </param>
    public DbProviderDescriptor(DbProviderFamily family, string? invariantName = null, DbProviderFactory? factory = null)
    {
        Family = family;
        InvariantName = invariantName;
        Factory = factory;
        
        // Default values for the generic provider
        DefaultPort = 0;
        DefaultDatabaseName = null;
        DefaultSchemaName = null;
        ObjectSeparator = ".";
        ParameterPrefixForStatement = "@";
        SupportsSchemas = false;
        SupportsNamedParameters = true;
        SupportsArrays = false;
        EnumSupport = DbEnumSupport.None;
        
        switch (Family)
        {
            case DbProviderFamily.Postgres:
                DefaultPort = 5432;
                DefaultDatabaseName = "postgres";
                DefaultSchemaName = "public";
                SupportsSchemas = true;
                SupportsNamedParameters = true;
                EnumSupport = DbEnumSupport.CustomType;
                SupportsArrays = true;
                break;
            case DbProviderFamily.MySql:
                DefaultPort = 3306;
                DefaultDatabaseName = "mysql";
                DefaultSchemaName = null;
                EnumSupport = DbEnumSupport.FieldRestriction;
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
        }
    }
    
    /// <summary>
    /// Database provider family.
    /// </summary>
    public DbProviderFamily Family { get; }
    
    /// <summary>
    /// Implementation name of the data provider used for communication with the database engine.
    /// </summary>
    public string? InvariantName { get; }
    
    /// <summary>
    /// Data provider factory used to create connections, commands, etc.
    /// </summary>
    public DbProviderFactory? Factory { get; }

    /// <summary>
    /// Default port used by the database management system.
    /// </summary>
    public int DefaultPort { get; }
    
    /// <summary>
    /// Default database name used by the database management system.
    /// </summary>
    public string? DefaultDatabaseName { get; }

    /// <summary>
    /// Default schema name used by the database management system.
    /// </summary>
    public string? DefaultSchemaName { get; }

    /// <summary>
    /// Object separator used in SQL statements.
    /// </summary>
    public string ObjectSeparator { get; }
    
    /// <summary>
    /// Parameter prefix used in SQL statements.
    /// Most providers use the '@' symbol,
    /// but some other providers like Oracle use ':'.
    /// </summary>
    public string ParameterPrefixForStatement { get; }
    
    /// <summary>
    /// Returns a value indicating whether the database engine supports schemas.
    /// </summary>
    public bool SupportsSchemas { get; }

    /// <summary>
    /// Returns a value indicating whether the database engine supports named parameters.
    /// </summary>
    public bool SupportsNamedParameters { get; }

    /// <summary>
    /// Returns a value indicating whether the database engine supports arrays as a data type.
    /// </summary>
    public bool SupportsArrays { get; }
    
    /// <summary>
    /// Returns a value indicating the type of support the database engine has for enumerations.
    /// </summary>
    public DbEnumSupport EnumSupport { get; }
    
    /// <summary>
    /// Returns a value indicating whether the database engine supports the specified routine type.
    /// </summary>
    /// <param name="routineType">
    /// Routine type to verify.
    /// </param>
    /// <returns>
    /// A boolean value indicating whether the database engine supports the specified routine type.
    /// </returns>
    public virtual bool SupportsRoutineType(DbRoutineType routineType)
     => Family != DbProviderFamily.Sqlite || routineType != DbRoutineType.StoredProcedure;
    
    /// <summary>
    /// Returns a value indicating whether the database engine supports the specified routine type and if it can return a table.
    /// </summary>
    /// <param name="routineType">
    /// Routine type to verify.
    /// </param>
    /// <returns>
    /// A boolean value indicating whether the database engine supports the specified routine type and if it can return a table.
    /// </returns>
    public virtual bool RoutineCanReturnTable(DbRoutineType routineType)
        => routineType == DbRoutineType.StoredProcedure || Family switch
        {
            DbProviderFamily.Postgres => true,
            DbProviderFamily.SqlServer => true,
            DbProviderFamily.Oracle => true,
            _ => false
        };
    
    /// <summary>
    /// Creates a fully qualified name for a database object.
    /// </summary>
    /// <param name="name">
    /// Name of the database object to qualify.
    /// </param>
    /// <param name="transform">
    /// Optional function that allows transforming each part of the object name.
    /// </param>
    /// <returns>
    /// A <see cref="DbFullyQualifiedName"/> object that represents the fully qualified name of the database object.
    /// </returns>
    public virtual DbFullyQualifiedName ParseObjectName(string name, Func<string, int, int, string>? transform = null)
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
    /// Formats a casting expression for a specific data type according to the database provider conventions.
    /// </summary>
    /// <param name="expression">
    /// Expression to format as a cast.
    /// </param>
    /// <param name="type">
    /// Data type to cast to.
    /// </param>
    /// <returns>
    /// A string that represents the casting expression formatted according to the database provider conventions.
    /// </returns>
    public string FormatCasting(string expression, string type)
        => Family switch
        {
            DbProviderFamily.Postgres => $"{expression}::{type}",
            _ => $"CAST({expression} AS {type})"
        };
    
    /// <summary>
    /// Formats a named parameter for a SQL statement.
    /// </summary>
    /// <param name="parameterName">
    /// Parameter name to format.
    /// </param>
    /// <param name="valueExpression">
    /// Expression that represents the parameter value.
    /// </param>
    /// <returns>
    /// A string that represents the named parameter formatted according to the database provider conventions.
    /// </returns>
    public virtual string FormatNamedParameter(string parameterName, string valueExpression)
        => Family switch
        {
            DbProviderFamily.Postgres => $"{parameterName} => {valueExpression}",
            DbProviderFamily.SqlServer => $"{parameterName} = {valueExpression}",
            _ => $"{ParameterPrefixForStatement}{parameterName}",
        };
    
    /// <summary>
    /// Gets the database data type corresponding to a runtime data type.
    /// </summary>
    /// <param name="runtimeType">
    /// Runtime data type for which to get the database data type.
    /// </param>
    /// <returns>
    /// A <see cref="DbType"/> value that represents the database data type corresponding to the runtime data type.
    /// </returns>
    public virtual DbType? InferDatabaseType(Type runtimeType)
    {
        var mappings = TypeMappings.GetOrAdd(Family, _ => new ConcurrentDictionary<Type, DbType>(GetTypeMappings()));
        return mappings.TryGetValue(runtimeType, out var dbType) ? dbType : null;
    }
    
    /// <summary>
    /// Formats a custom type name as its corresponding array type.
    /// </summary>
    /// <param name="databaseCustomType">
    /// Custom database data type name to format as an array type.
    /// </param>
    /// <returns>
    /// A string that represents the custom data type formatted as an array type.
    /// </returns>
    public virtual string? FormatArrayType(string? databaseCustomType)
        => !string.IsNullOrEmpty(databaseCustomType) && Family == DbProviderFamily.Postgres
            ? $"{databaseCustomType}[]"
            : databaseCustomType;
    
    /// <summary>
    /// Formats a SQL statement for routine (stored procedure or function) invocation.
    /// </summary>
    /// <param name="fullyQualifiedName">
    /// Fully qualified name of the routine to invoke.
    /// </param>
    /// <param name="routineType">
    /// Type of routine to invoke (stored procedure or function).
    /// </param>
    /// <param name="useNamedParameters">
    /// Indicates whether named parameters should be used in the routine invocation.
    /// </param>
    /// <param name="returnsTable">
    /// Indicates whether the routine returns a table.
    /// </param>
    /// <param name="parameters">
    /// Parameters to pass to the routine.
    /// </param>
    /// <returns>
    /// A string that represents the SQL statement formatted for routine invocation.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// Thrown if the routine type is not supported by the database provider,
    /// or if attempting to return a table from a routine type that does not support it.
    /// </exception>
    public virtual string FormatRoutineCall(string fullyQualifiedName, DbRoutineType routineType, bool useNamedParameters = false, bool returnsTable = false, params DbParameterDescriptor[] parameters)
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
    
    /// <summary>
    /// Gets a data type mapping dictionary.
    /// </summary>
    /// <returns>
    /// A dictionary that maps runtime data types to database data types <see cref="DbType"/>.
    /// </returns>
    protected virtual IDictionary<Type, DbType> GetTypeMappings()
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
            [typeof(DateTime)] = DbType.DateTime2,
            [typeof(DateTimeOffset)] = DbType.DateTimeOffset,
            [typeof(byte[])] = DbType.Binary
        };

        // Add provider-specific overrides here if needed
        switch (Family)
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
