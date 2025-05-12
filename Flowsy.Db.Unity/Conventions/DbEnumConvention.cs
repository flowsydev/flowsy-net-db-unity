using System.Collections.Concurrent;
using System.Data;
using Flowsy.Core;
using Flowsy.Db.Unity.Resources;

namespace Flowsy.Db.Unity.Conventions;

public class DbEnumConvention : DbConvention
{
    internal DbEnumConvention(DbConventionSet conventions) : base(conventions)
    {
    }

    public DbEnumFormat ValueFormat { get; internal set; }
    
    public DbEnumNameTranslator NameTranslator { get; internal set; } = new ();
    
    private readonly ConcurrentDictionary<Type, DbEnumMapping> _mappings = [];
    public IEnumerable<DbEnumMapping> Mappings => _mappings.Values;
    
    internal void AddMapping(DbEnumMapping mapping)
    {
        if (!_mappings.TryAdd(mapping.RuntimeType, mapping))
            throw new InvalidOperationException(string.Format(Strings.MappingForEnumTypeXAlreadyExists, mapping.RuntimeType));
    }
    
    public DbEnumMapping? ResolveMapping(Type runtimeType) => _mappings.GetValueOrDefault(runtimeType);
    
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

    public void Map(Type enumType, out DbType databaseType, out string? customType, out DbEnumMapping? mapping)
    {
        mapping = null;
        
        if (ValueFormat == DbEnumFormat.Ordinal)
        {
            databaseType = enumType.GetEnumValues().GetValue(0) is Enum firstValue 
                ? ResolveOrdinalType(firstValue) 
                : DbType.String;
            customType = null;
            return;
        }
        
        databaseType = DbType.String;
        customType = null;
        
        mapping = ResolveMapping(enumType);
        if (mapping is null)
            return;
        
        var nameTranslator = mapping.NameTranslator ?? NameTranslator;
        customType = mapping.DatabaseTypeName?.ToString() ?? nameTranslator.TranslateTypeName(enumType);
    }

    public void Map(Enum @enum, out DbType databaseType, out string? customType, out object enumValue)
    {
        Map(@enum.GetType(), out databaseType, out customType, out var mapping);

        var enumStringValue = @enum.ToString();
        var nameTranslator = mapping?.NameTranslator ?? NameTranslator;
        if (nameTranslator.MemberNameCaseStyle.HasValue)
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

    public void CopyTo(DbEnumConvention other)
    {
        other.ValueFormat = ValueFormat;
        other.NameTranslator = NameTranslator;
        
        foreach (var mapping in Mappings)
        {
            var newMapping = new DbEnumMapping(mapping.RuntimeType, mapping.DatabaseTypeName, mapping.NameTranslator, Conventions);
            other.AddMapping(newMapping);
        }
    }

    public DbEnumConvention Clone(DbConventionSet parentConventions)
    {
        var clone = new DbEnumConvention(parentConventions);
        CopyTo(clone);
        return clone;
    }
}