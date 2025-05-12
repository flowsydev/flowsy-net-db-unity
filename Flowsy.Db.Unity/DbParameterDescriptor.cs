using System.Collections;
using System.Data;
using Flowsy.Db.Unity.Conventions;

namespace Flowsy.Db.Unity;

/// <summary>
/// Describes a database parameter.
/// </summary>
public sealed class DbParameterDescriptor : DbObjectDescriptor
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
    /// The parameter runtime type.
    /// </param>
    /// <param name="databaseType">
    /// The parameter database type.
    /// </param>
    /// <param name="customType">
    /// The parameter custom type.
    /// </param>
    /// <param name="valueExpression">
    /// The parameter value expression.
    /// </param>
    /// <param name="direction">
    /// The parameter direction.
    /// </param>
    /// <param name="size">
    /// The parameter size.
    /// </param>
    /// <param name="precision">
    /// The parameter precision.
    /// </param>
    /// <param name="scale">
    /// The parameter scale.
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
    /// The parameter value expression.
    /// </summary>
    public DbValueExpression ValueExpression { get; }
    
    /// <summary>
    /// The parameter runtime type.
    /// </summary>
    public Type RuntimeType { get; }
    
    /// <summary>
    /// The parameter database type.
    /// </summary>
    public DbType? DatabaseType { get; }
    
    public string? CustomType { get; }
    
    /// <summary>
    /// The parameter direction.
    /// </summary>
    public ParameterDirection? Direction { get; }
    
    /// <summary>
    /// The parameter size.
    /// </summary>
    public int? Size { get; }
    
    /// <summary>
    /// The parameter precision.
    /// </summary>
    public byte? Precision { get; }
    
    /// <summary>
    /// The parameter scale.
    /// </summary>
    public byte? Scale { get; }

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
                databaseValue = (
                    from object? value in enumerable
                    select ResolveDatabaseValue(value, conventions)
                ).ToArray();
                break;
            
            default:
                databaseValue = runtimeValue;
                break;
        }
        
        return databaseValue;
    }
}