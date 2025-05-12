using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using Flowsy.Db.Unity.Resources;

namespace Flowsy.Db.Unity;

public class DbProviderDescriptor
{
    public static readonly DbProviderDescriptor Generic = new (DbProviderFamily.Generic);
    private static readonly ConcurrentDictionary<DbProviderFamily, ConcurrentDictionary<Type, DbType>> TypeMappings = [];
    
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

    public DbProviderFamily Family { get; }
    public string? InvariantName { get; }
    public DbProviderFactory? Factory { get; }

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
    /// Gets the default port for the provider.
    /// </summary>
    public int DefaultPort { get; }
    
    /// <summary>
    /// Gets the default database name for the provider.
    /// </summary>
    public string? DefaultDatabaseName { get; }
    
    /// <summary>
    /// Gets the default schema for the provider.
    /// </summary>
    public string? DefaultSchemaName { get; }
    
    /// <summary>
    /// Gets the parameter prefix for a statement.
    /// </summary>
    public string ParameterPrefixForStatement { get; }
    
    /// <summary>
    /// Gets a value indicating whether the provider supports schemas.
    /// </summary>
    public bool SupportsSchemas { get; }
    
    /// <summary>
    /// Gets a value indicating whether the provider supports named parameters.
    /// </summary>
    public bool SupportsNamedParameters { get; }
    
    /// <summary>
    /// Gets a value indicating whether the provider supports enums.
    /// </summary>
    public bool SupportsEnums { get; }
    
    /// <summary>
    /// Gets a value indicating whether the provider supports enums as custom types.
    /// </summary>
    public bool SupportsEnumsAsCustomTypes { get; }
    
    public bool SupportsArrays { get; }
    
    public bool SupportsRoutineType(DbRoutineType routineType)
        => Family != DbProviderFamily.Sqlite || routineType != DbRoutineType.StoredProcedure;
    
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
    
    public string FormatCasting(string expression, string type)
        => Family switch
        {
            DbProviderFamily.Postgres => $"{expression}::{type}",
            _ => $"CAST({expression} AS {type})"
        };
    
    public string FormatNamedParameter(string parameterName, string valueExpression)
        => Family switch
        {
            DbProviderFamily.Postgres => $"{parameterName} => {valueExpression}",
            DbProviderFamily.SqlServer => $"{parameterName} = {valueExpression}",
            _ => $"{ParameterPrefixForStatement}{parameterName}",
        };

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

    public string FormatRoutineCall(DbRoutineDescriptor routine)
        => FormatRoutineCall(routine.FullyQualifiedName.ToString(), routine.Type, routine.UseNamedParameters, routine.ReturnsTable, routine.Parameters.ToArray());
    
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
    
    public DbType? GetDatabaseType(Type runtimeType)
    {
        var mappings = TypeMappings.GetOrAdd(Family, CreateDefaultTypeMappings);
        return mappings?.TryGetValue(runtimeType, out var dbType) ?? false ? dbType : null;
    }

    public string? FormatArrayType(string? databaseCustomType)
        => Family == DbProviderFamily.Postgres ? $"{databaseCustomType}[]" : null;
}