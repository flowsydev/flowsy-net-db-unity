using System.Collections;
using System.Data;
using Flowsy.Db.Unity.Conventions;

namespace Flowsy.Db.Unity;

/// <summary>
/// Represents a database parameter.
/// </summary>
public class DbParameterDescriptor : DbObjectDescriptor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DbParameterDescriptor"/> class.
    /// </summary>
    /// <param name="provider">
    /// The database provider descriptor.
    /// </param>
    /// <param name="name">
    /// The parameter name.
    /// </param>
    /// <param name="runtimeType">
    /// The parameter type within the .NET runtime engine.
    /// </param>
    /// <param name="databaseType">
    /// The parameter type within the database engine.
    /// </param>
    /// <param name="customType">
    /// The custom parameter type, if applicable.
    /// </param>
    /// <param name="valueExpression">
    /// The value expression to use for the parameter.
    /// </param>
    /// <param name="direction">
    /// The parameter direction (input, output, etc.).
    /// </param>
    /// <param name="size">
    /// The parameter size, if applicable.
    /// </param>
    /// <param name="precision">
    /// The parameter precision, if applicable.
    /// </param>
    /// <param name="scale">
    /// The parameter scale, if applicable.
    /// </param>
    public DbParameterDescriptor(
        DbProviderDescriptor provider,
        string name,
        Type runtimeType,
        DbType? databaseType = null,
        string? customType = null,
        DbValueExpression valueExpression = DbValueExpression.Raw,
        ParameterDirection? direction = null,
        int? size = null,
        byte? precision = null,
        byte? scale = null
    ) : base(provider.ParseObjectName(name))
    {
        Name = name;
        RuntimeType = runtimeType;
        DatabaseType = databaseType;
        CustomType = customType;
        ValueExpression = valueExpression;
        Direction = direction;
        Size = size;
        Precision = precision;
        Scale = scale;
    }

    /// <summary>
    /// The parameter name.
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// The value expression to use for the parameter.
    /// </summary>
    public DbValueExpression ValueExpression { get; }
    
    /// <summary>
    /// The parameter type within the .NET runtime engine.
    /// </summary>
    public Type RuntimeType { get; }
    
    /// <summary>
    /// The parameter type within the database engine.
    /// </summary>
    public DbType? DatabaseType { get; }
    
    /// <summary>
    /// The custom parameter type, if applicable.
    /// </summary>
    public string? CustomType { get; }
    
    /// <summary>
    /// The parameter direction (input, output, etc.).
    /// </summary>
    public ParameterDirection? Direction { get; }
    
    /// <summary>
    /// The parameter size, if applicable.
    /// </summary>
    public int? Size { get; }
    
    /// <summary>
    /// The parameter precision, if applicable.
    /// </summary>
    public byte? Precision { get; }
    
    /// <summary>
    /// The parameter scale, if applicable.
    /// </summary>
    public byte? Scale { get; }

    /// <summary>
    /// Resolves the database value for the parameter based on the runtime value and conventions.
    /// </summary>
    /// <param name="runtimeValue">
    /// The runtime value to convert to a database value.
    /// </param>
    /// <param name="conventions">
    /// Optional conventions to use for value conversion. If null, default conversion rules apply.
    /// </param>
    /// <returns>
    /// The converted database value, or null if the runtime value is null.
    /// </returns>
    public object? ResolveDatabaseValue(object? runtimeValue, DbConventionSet? conventions = null)
    {
        if (runtimeValue is null)
            return null;

        object databaseValue;
        
        switch (runtimeValue)
        {
            case Enum e:
                if (conventions is not null)
                {
                    conventions.Enums.Map(e, out _, out _, out var enumValue);
                    databaseValue = enumValue;
                }
                else
                    databaseValue = e.ToString();
                break;

            case DateTime dt:
                databaseValue = dt.Kind != DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(dt, DateTimeKind.Unspecified)
                    : dt;
                break;
            
            case DateTimeOffset dto:
                if (conventions is not null)
                {
                    databaseValue = conventions.DateTime.OffsetValueFormat == DbDateTimeOffsetFormat.Utc
                        ? dto.UtcDateTime
                        : dto.LocalDateTime;
                }
                else
                    databaseValue = dto.UtcDateTime;
                break;
            
            case string s:
                databaseValue = s;
                break;
            
            case IEnumerable enumerable:
            {
                // Return value as array of the underlying type
                var elementType = RuntimeType.IsArray
                    ? RuntimeType.GetElementType()!
                    : RuntimeType.GenericTypeArguments.Length > 0
                        ? RuntimeType.GenericTypeArguments[0]
                        : typeof(object);
                
                var list = new ArrayList();
                foreach (var item in enumerable)
                    list.Add(Convert.ChangeType(item, elementType));
               
                databaseValue = list.ToArray(elementType);
                break;
            }
            
            default:
                databaseValue = runtimeValue;
                break;
        }
        
        return databaseValue;
    }
}
