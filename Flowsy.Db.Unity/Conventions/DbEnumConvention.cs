using System.Collections.Concurrent;
using System.Data;
using Flowsy.Db.Unity.Resources;

namespace Flowsy.Db.Unity.Conventions;

/// <summary>
/// Represents conventions for database enum types.
/// </summary>
/// <param name="ValueFormat">The format to use for enum values in the database.</param>
/// <param name="NameTranslator">Optional name translator for enum types and members.</param>
/// <param name="Mappings">Optional specific mappings for enum types.</param>
public record DbEnumConvention(
    DbEnumValueFormat ValueFormat,
    DbEnumNameTranslator? NameTranslator = null,
    IEnumerable<DbEnumMapping>? Mappings = null
) : DbConvention
{
    /// <summary>
    /// Default enum convention that uses name format for enum values.
    /// </summary>
    public static readonly DbEnumConvention Default = new(DbEnumValueFormat.Name, null);
    
    private ConcurrentDictionary<Type, DbEnumMapping>? _dictionary;
    /// <summary>
    /// Gets a dictionary that caches enum mappings for performance.
    /// </summary>
    private ConcurrentDictionary<Type, DbEnumMapping> Dictionary
    {
        get
        {
            if (_dictionary is not null)
                return _dictionary;
            
            var d = new ConcurrentDictionary<Type, DbEnumMapping>();
            foreach (var mapping in Mappings ?? [])
            {
                if (!d.TryAdd(mapping.RuntimeType, mapping))
                    throw new InvalidOperationException(string.Format(Strings.MappingForEnumTypeXAlreadyExists, mapping.RuntimeType));
            }
            _dictionary = d;
            return _dictionary;
        }
    }
    
    /// <summary>
    /// Resolves the mapping for the specified enum type.
    /// </summary>
    /// <param name="runtimeType">The enum type to resolve mapping for.</param>
    /// <returns>The enum mapping if found; otherwise, null.</returns>
    public DbEnumMapping? ResolveMapping(Type runtimeType) => Dictionary.GetValueOrDefault(runtimeType);
    
    /// <summary>
    /// Resolves the mapping for the specified enum type.
    /// </summary>
    /// <typeparam name="TEnum">The enum type to resolve mapping for.</typeparam>
    /// <returns>The enum mapping if found; otherwise, null.</returns>
    public DbEnumMapping? ResolveMapping<TEnum>() where TEnum : struct, Enum => ResolveMapping(typeof(TEnum));
    
    /// <summary>
    /// Resolves the database type for an enum when using ordinal format.
    /// </summary>
    /// <param name="e">The enum value to resolve type for.</param>
    /// <returns>The corresponding DbType for the enum's underlying type.</returns>
    private DbType ResolveOrdinalType(Enum e)
        => e.GetTypeCode() switch
        {
            TypeCode.Byte => DbType.Byte,
            TypeCode.Int16 => DbType.Int16,
            TypeCode.Int64 => DbType.Int64,
            _ => DbType.Int32
        };

    /// <summary>
    /// Resolves the database value for an enum when using ordinal format.
    /// </summary>
    /// <param name="e">The enum value to resolve value for.</param>
    /// <returns>The ordinal value of the enum converted to its underlying type.</returns>
    private object ResolveOrdinalValue(Enum e)
        => e.GetTypeCode() switch
        {
            TypeCode.Byte => Convert.ToByte(e),
            TypeCode.Int16 => Convert.ToInt16(e),
            TypeCode.Int64 => Convert.ToInt64(e),
            _ => Convert.ToInt32(e)
        };

    /// <summary>
    /// Maps an enum type to its database representation, determining the database type, custom type name, and mapping configuration.
    /// </summary>
    /// <param name="enumType">The enum type to map.</param>
    /// <param name="databaseType">When this method returns, contains the database type for the enum.</param>
    /// <param name="customType">When this method returns, contains the custom database type name if applicable; otherwise, null.</param>
    /// <param name="mapping">When this method returns, contains the enum mapping if found; otherwise, null.</param>
    public void Map(Type enumType, out DbType databaseType, out string? customType, out DbEnumMapping? mapping)
    {
        mapping = ResolveMapping(enumType);
        customType = null;
        
        if (ValueFormat == DbEnumValueFormat.Ordinal)
        {
            var enumValues = Enum.GetValues(enumType);
            var firstValueAsObject = enumValues.Length > 0 ? enumValues.GetValue(0) : null;
            
            databaseType = firstValueAsObject is Enum firstValue ? ResolveOrdinalType(firstValue) : DbType.Int32;
            return;
        }
        
        databaseType = DbType.String;
        
        if (mapping is null)
            return;

        if (mapping.DatabaseTypeName is not null)
        {
            customType = mapping.DatabaseTypeName.ToString();
            return;
        }
        
        var nameTranslator = mapping.NameTranslator ?? NameTranslator;
        if (nameTranslator is null)
            return;

        customType = nameTranslator.TranslateTypeName(enumType);
    }

    /// <summary>
    /// Maps an enum value to its database representation, determining the database type, custom type name, and the actual database value.
    /// </summary>
    /// <param name="enum">The enum value to map.</param>
    /// <param name="databaseType">When this method returns, contains the database type for the enum.</param>
    /// <param name="customType">When this method returns, contains the custom database type name if applicable; otherwise, null.</param>
    /// <param name="enumValue">When this method returns, contains the database value for the enum.</param>
    public void Map(Enum @enum, out DbType databaseType, out string? customType, out object enumValue)
    {
        Map(@enum.GetType(), out databaseType, out customType, out var mapping);

        var enumStringValue = @enum.ToString();
        var nameTranslator = mapping?.NameTranslator ?? NameTranslator;
        if (nameTranslator?.MemberNameCaseStyle.HasValue ?? false)
            enumValue = nameTranslator.TranslateMemberName(enumStringValue);
        else
        {
            var defaultCaseStyle = ConventionSet?.DefaultCaseStyle;
            if (defaultCaseStyle.HasValue && defaultCaseStyle.Value != DbCaseStyle.None)
                enumValue = defaultCaseStyle.Value.Apply(enumStringValue);
            else
                enumValue = enumStringValue;
        }
    }
}