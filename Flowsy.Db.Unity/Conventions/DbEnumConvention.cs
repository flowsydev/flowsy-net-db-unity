using System.Collections.Concurrent;
using System.Data;
using Flowsy.Core;
using Flowsy.Db.Unity.Resources;

namespace Flowsy.Db.Unity.Conventions;

/// <summary>
/// Represents a convention for handling enumerations in database operations.
/// </summary>
public class DbEnumConvention : DbConvention
{
    private readonly ConcurrentDictionary<Type, DbEnumMapping> _mappings = [];
    
    internal DbEnumConvention(DbConventionSet conventions) : base(conventions)
    {
    }

    /// <summary>
    /// The format used to pass enum values to the database.
    /// </summary>
    public DbEnumFormat ValueFormat { get; internal set; }
    
    /// <summary>
    /// The translator used to convert runtime names to database names for enum types and their members.
    /// </summary>
    public DbEnumNameTranslator? NameTranslator { get; internal set; } = new ();
    
    /// <summary>
    /// Adds a mapping for a specific enum type to its database representation.
    /// </summary>
    /// <param name="mapping">
    /// The mapping to be added.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if a mapping for the specified enum type already exists in the collection.
    /// </exception>
    internal void AddMapping(DbEnumMapping mapping)
    {
        if (!_mappings.TryAdd(mapping.RuntimeType, mapping))
            throw new InvalidOperationException(string.Format(Strings.MappingForEnumTypeXAlreadyExists, mapping.RuntimeType));
    }
    
    /// <summary>
    /// Resolves the mapping for a specific enum type.
    /// </summary>
    /// <param name="runtimeType">
    /// The runtime type of the enum for which to resolve the mapping.
    /// </param>
    /// <returns>
    /// The mapping for the specified enum type, or null if no mapping exists.
    /// </returns>
    public DbEnumMapping? ResolveMapping(Type runtimeType) => _mappings.GetValueOrDefault(runtimeType);
    
    /// <summary>
    /// Resolves the mapping for a specific enum type.
    /// </summary>
    /// <typeparam name="TEnum">
    /// The enum type for which to resolve the mapping.
    /// </typeparam>
    /// <returns>
    /// The mapping for the specified enum type, or null if no mapping exists.
    /// </returns>
    public DbEnumMapping? ResolveMapping<TEnum>() where TEnum : struct, Enum => ResolveMapping(typeof(TEnum));
    
    private DbType ResolveOrdinalType(Enum e)
        => e.GetTypeCode() switch
        {
            TypeCode.Byte => DbType.Byte,
            TypeCode.Int16 => DbType.Int16,
            TypeCode.Int64 => DbType.Int64,
            _ => DbType.Int32
        };

    private object ResolveOrdinalValue(Enum e)
        => e.GetTypeCode() switch
        {
            TypeCode.Byte => Convert.ToByte(e),
            TypeCode.Int16 => Convert.ToInt16(e),
            TypeCode.Int64 => Convert.ToInt64(e),
            _ => Convert.ToInt32(e)
        };

    /// <summary>
    /// Maps an enum type to its database representation.
    /// </summary>
    /// <param name="enumType">
    /// The type of the enum to be mapped.
    /// </param>
    /// <param name="databaseType">
    /// The database type to which the enum type is mapped.
    /// This will be <see cref="DbType.String"/> if the convention is set to <see cref="DbEnumFormat.Name"/>, or the underlying type of the enum if the convention is set to <see cref="DbEnumFormat.Ordinal"/>.
    /// </param>
    /// <param name="customType">
    /// The custom type name for the database representation of the enum type.
    /// The custom type name will be resolved using the registered mapping's custom name, its name translator or the default name translator, in that order.
    /// </param>
    /// <param name="mapping">
    /// The mapping for the enum type, if it exists.
    /// </param>
    public void Map(Type enumType, out DbType databaseType, out string? customType, out DbEnumMapping? mapping)
    {
        mapping = ResolveMapping(enumType);
        customType = null;
        
        if (ValueFormat == DbEnumFormat.Ordinal)
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
    /// Maps an enum value to its database representation.
    /// </summary>
    /// <param name="enum">
    /// The enum value to be mapped.
    /// </param>
    /// <param name="databaseType">
    /// The database type to which the enum type is mapped.
    /// This will be <see cref="DbType.String"/> if the convention is set to <see cref="DbEnumFormat.Name"/>, or the underlying type of the enum if the convention is set to <see cref="DbEnumFormat.Ordinal"/>.
    /// </param>
    /// <param name="customType">
    /// The custom type name for the database representation of the enum type.
    /// The custom type name will be resolved using the registered mapping's custom name, its name translator or the default name translator, in that order.
    /// </param>
    /// <param name="enumValue">
    /// The value of the enum as it should be passed to the database.
    /// </param>
    public void Map(Enum @enum, out DbType databaseType, out string? customType, out object enumValue)
    {
        Map(@enum.GetType(), out databaseType, out customType, out var mapping);

        var enumStringValue = @enum.ToString();
        var nameTranslator = mapping?.NameTranslator ?? NameTranslator;
        if (nameTranslator?.MemberNameCaseStyle.HasValue ?? false)
            enumValue = nameTranslator.TranslateMemberName(enumStringValue);
        else
        {
            var defaultCaseStyle = Conventions.DefaultCaseStyle;
            if (defaultCaseStyle.HasValue && !enumStringValue.MatchesCaseStyle(defaultCaseStyle.Value))
                enumValue = enumStringValue.ApplyCaseStyle(defaultCaseStyle.Value);
            else
                enumValue = enumStringValue;
        }
    }

    /// <summary>
    /// Copies the properties of this <see cref="DbEnumConvention"/> instance to another instance.
    /// </summary>
    /// <param name="other">
    /// The other <see cref="DbEnumConvention"/> instance to copy properties to.
    /// </param>
    public void CopyTo(DbEnumConvention other)
    {
        other.ValueFormat = ValueFormat;
        other.NameTranslator = NameTranslator;
        
        foreach (var mapping in _mappings.Values)
        {
            var newMapping = new DbEnumMapping(mapping.RuntimeType, mapping.DatabaseTypeName, mapping.NameTranslator, Conventions);
            other.AddMapping(newMapping);
        }
    }

    /// <summary>
    /// Creates a clone of this <see cref="DbEnumConvention"/> instance.
    /// </summary>
    /// <param name="parentConventions">
    /// The parent <see cref="DbConventionSet"/> to which the cloned convention will belong.
    /// </param>
    /// <returns>
    /// A new instance of <see cref="DbEnumConvention"/> with the same properties as this instance.
    /// </returns>
    public DbEnumConvention Clone(DbConventionSet parentConventions)
    {
        var clone = new DbEnumConvention(parentConventions);
        CopyTo(clone);
        return clone;
    }
}