using Flowsy.Core;

namespace Flowsy.Db.Unity.Conventions;

public class DbEnumConventionBuilder : DbConventionBuilder
{
    public DbEnumConventionBuilder(DbConventionSetBuilder parent) : base(parent)
    {
    }

    public DbEnumConventionBuilder Use(DbEnumFormat valueFormat, CaseStyle? memberNameCaseStyle = null, CaseStyle? typeNameCaseStyle = null, IEnumerable<DbEnumMapping>? mappings = null)
    {
        var nameTranslator = memberNameCaseStyle.HasValue || typeNameCaseStyle.HasValue
            ? new DbEnumNameTranslator(memberNameCaseStyle, typeNameCaseStyle)
            : null;
        return Use(valueFormat, nameTranslator, mappings?.ToArray() ?? []);
    }
    
    public DbEnumConventionBuilder Use(DbEnumFormat valueFormat, DbEnumNameTranslator? nameTranslator = null, IEnumerable<DbEnumMapping>? mappings = null)
    {
        Parent.Conventions.Enums.ValueFormat = valueFormat;
        if (nameTranslator is not null)
            Parent.Conventions.Enums.NameTranslator = nameTranslator;

        if (mappings == null) return this;
        
        foreach (var mapping in mappings)
            Parent.Conventions.Enums.AddMapping(mapping);
        
        return this;
    }

    public DbEnumConventionBuilder UseValueFormat(DbEnumFormat valueFormat)
    {
        Parent.Conventions.Enums.ValueFormat = valueFormat;
        return this;
    }
    
    public DbEnumConventionBuilder UseNames(CaseStyle? memberNameCaseStyle = null, CaseStyle? typeNameCaseStyle = null)
        => UseNames(!memberNameCaseStyle.HasValue && !typeNameCaseStyle.HasValue ? null : new DbEnumNameTranslator(memberNameCaseStyle, typeNameCaseStyle));
    
    public DbEnumConventionBuilder UseNames(DbEnumNameTranslator? nameTranslator)
    {
        Parent.Conventions.Enums.NameTranslator = nameTranslator;
        return this;
    }

    public DbEnumConventionBuilder UseMapping<TEnum>(string? databaseTypeName = null, CaseStyle? memberNameCaseStyle = null, CaseStyle? typeNameCaseStyle = null) where TEnum : struct, Enum
    {
        var nameTranslator = memberNameCaseStyle.HasValue || typeNameCaseStyle.HasValue
            ? new DbEnumNameTranslator(memberNameCaseStyle, typeNameCaseStyle)
            : null;
        
        Parent.Conventions.Enums.AddMapping(new DbEnumMapping<TEnum>(databaseTypeName, nameTranslator, Parent.Conventions));
        return this;
    }

    public DbEnumConventionBuilder UseMapping(Type runtimeType, string? databaseTypeName = null, CaseStyle? memberNameCaseStyle = null, CaseStyle? typeNameCaseStyle = null)
    {
        var nameTranslator = memberNameCaseStyle.HasValue || typeNameCaseStyle.HasValue
            ? new DbEnumNameTranslator(typeNameCaseStyle, memberNameCaseStyle)
            : null;
        
        Parent.Conventions.Enums.AddMapping(new DbEnumMapping(runtimeType, databaseTypeName, nameTranslator, Parent.Conventions));
        return this;
    }

    public DbEnumConventionBuilder UseMappings(params DbEnumMapping[] mappings)
    {
        foreach (var mapping in mappings)
            Parent.Conventions.Enums.AddMapping(mapping);
        
        return this;
    }
}