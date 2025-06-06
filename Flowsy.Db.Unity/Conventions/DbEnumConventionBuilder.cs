using Flowsy.Core;

namespace Flowsy.Db.Unity.Conventions;

/// <summary>
/// Builder for configuring database enum conventions.
/// </summary>
public class DbEnumConventionBuilder : DbConventionBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DbEnumConventionBuilder"/> class.
    /// </summary>
    /// <param name="parent">
    /// The parent <see cref="DbConventionSetBuilder"/> instance.
    /// </param>
    internal DbEnumConventionBuilder(DbConventionSetBuilder parent) : base(parent)
    {
    }

    /// <summary>
    /// Sets the enum value format and optional name translator for the target <see cref="DbEnumConvention"/> instance.
    /// </summary>
    /// <param name="valueFormat">
    /// The format to be used for enum values. This can be one of the values from the <see cref="DbEnumFormat"/> enumeration.
    /// </param>
    /// <param name="memberNameCaseStyle">
    /// The case style to be used for enum member names. This can be null, in which case no transformation will be applied when resolving names.
    /// </param>
    /// <param name="typeNameCaseStyle">
    /// The case style to be used for enum type names. This can be null, in which case no transformation will be applied when resolving names.
    /// </param>
    /// <param name="mappings">
    /// An optional collection of <see cref="DbEnumMapping"/> instances to be used for mapping enum types to database types.
    /// </param>
    /// <returns>
    /// A reference to the current <see cref="DbEnumConventionBuilder"/> instance, allowing for method chaining.
    /// </returns>
    public DbEnumConventionBuilder Use(DbEnumFormat valueFormat, CaseStyle? memberNameCaseStyle = null, CaseStyle? typeNameCaseStyle = null, IEnumerable<DbEnumMapping>? mappings = null)
    {
        var nameTranslator = memberNameCaseStyle.HasValue || typeNameCaseStyle.HasValue
            ? new DbEnumNameTranslator(memberNameCaseStyle, typeNameCaseStyle)
            : null;
        return Use(valueFormat, nameTranslator, mappings?.ToArray() ?? []);
    }
    
    /// <summary>
    /// Sets the enum value format and optional name translator for the target <see cref="DbEnumConvention"/> instance.
    /// </summary>
    /// <param name="valueFormat">
    /// The format to be used for enum values. This can be one of the values from the <see cref="DbEnumFormat"/> enumeration.
    /// </param>
    /// <param name="nameTranslator">
    /// An optional <see cref="DbEnumNameTranslator"/> instance to be used for translating enum names.
    /// </param>
    /// <param name="mappings">
    /// An optional collection of <see cref="DbEnumMapping"/> instances to be used for mapping enum types to database types.
    /// </param>
    /// <returns>
    /// A reference to the current <see cref="DbEnumConventionBuilder"/> instance, allowing for method chaining.
    /// </returns>
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

    /// <summary>
    /// Sets the enum value format for the target <see cref="DbEnumConvention"/> instance.
    /// </summary>
    /// <param name="valueFormat">
    /// The format to be used for enum values. This can be one of the values from the <see cref="DbEnumFormat"/> enumeration.
    /// </param>
    /// <returns>
    /// A reference to the current <see cref="DbEnumConventionBuilder"/> instance, allowing for method chaining.
    /// </returns>
    public DbEnumConventionBuilder UseValueFormat(DbEnumFormat valueFormat)
    {
        Parent.Conventions.Enums.ValueFormat = valueFormat;
        return this;
    }
    
    /// <summary>
    /// Sets the name translator for the target <see cref="DbEnumConvention"/> instance.
    /// </summary>
    /// <param name="memberNameCaseStyle">
    /// The case style to be used for enum member names. This can be null, in which case no transformation will be applied when resolving names.
    /// </param>
    /// <param name="typeNameCaseStyle">
    /// The case style to be used for enum type names. This can be null, in which case no transformation will be applied when resolving names.
    /// </param>
    /// <returns>
    /// A reference to the current <see cref="DbEnumConventionBuilder"/> instance, allowing for method chaining.
    /// </returns>
    public DbEnumConventionBuilder UseNames(CaseStyle? memberNameCaseStyle = null, CaseStyle? typeNameCaseStyle = null)
        => UseNames(!memberNameCaseStyle.HasValue && !typeNameCaseStyle.HasValue ? null : new DbEnumNameTranslator(memberNameCaseStyle, typeNameCaseStyle));
    
    /// <summary>
    /// Sets the name translator for the target <see cref="DbEnumConvention"/> instance.
    /// </summary>
    /// <param name="nameTranslator">
    /// An optional <see cref="DbEnumNameTranslator"/> instance to be used for translating enum names.
    /// If null, no transformation will be applied when resolving names.
    /// </param>
    /// <returns>
    /// A reference to the current <see cref="DbEnumConventionBuilder"/> instance, allowing for method chaining.
    /// </returns>
    public DbEnumConventionBuilder UseNames(DbEnumNameTranslator? nameTranslator)
    {
        Parent.Conventions.Enums.ValueFormat = DbEnumFormat.Name;
        Parent.Conventions.Enums.NameTranslator = nameTranslator;
        return this;
    }

    /// <summary>
    /// Adds a mapping for a specific enum type to a database type.
    /// </summary>
    /// <param name="databaseTypeName">
    /// The name of the database type to which the enum type will be mapped. If null, the default database type will be used.
    /// </param>
    /// <param name="memberNameCaseStyle">
    /// The case style to be used for enum member names. This can be null, in which case no transformation will be applied when resolving names.
    /// </param>
    /// <param name="typeNameCaseStyle">
    /// The case style to be used for enum type names. This can be null, in which case no transformation will be applied when resolving names.
    /// </param>
    /// <typeparam name="TEnum">
    /// The enum type to be mapped. This must be a value type and an enum.
    /// </typeparam>
    /// <returns>
    /// A reference to the current <see cref="DbEnumConventionBuilder"/> instance, allowing for method chaining.
    /// </returns>
    public DbEnumConventionBuilder UseMapping<TEnum>(string? databaseTypeName = null, CaseStyle? memberNameCaseStyle = null, CaseStyle? typeNameCaseStyle = null) where TEnum : struct, Enum
    {
        var nameTranslator = memberNameCaseStyle.HasValue || typeNameCaseStyle.HasValue
            ? new DbEnumNameTranslator(memberNameCaseStyle, typeNameCaseStyle)
            : null;
        
        Parent.Conventions.Enums.AddMapping(new DbEnumMapping<TEnum>(databaseTypeName, nameTranslator, Parent.Conventions));
        return this;
    }

    /// <summary>
    /// Adds a mapping for a specific enum type to a database type.
    /// </summary>
    /// <param name="runtimeType">
    /// The runtime type of the enum to be mapped. This must be a value type and an enum.
    /// </param>
    /// <param name="databaseTypeName">
    /// The name of the database type to which the enum type will be mapped. If null, the default database type will be used.
    /// </param>
    /// <param name="memberNameCaseStyle">
    /// The case style to be used for enum member names. This can be null, in which case no transformation will be applied when resolving names.
    /// </param>
    /// <param name="typeNameCaseStyle">
    /// The case style to be used for enum type names. This can be null, in which case no transformation will be applied when resolving names.
    /// </param>
    /// <returns>
    /// A reference to the current <see cref="DbEnumConventionBuilder"/> instance, allowing for method chaining.
    /// </returns>
    public DbEnumConventionBuilder UseMapping(Type runtimeType, string? databaseTypeName = null, CaseStyle? memberNameCaseStyle = null, CaseStyle? typeNameCaseStyle = null)
    {
        var nameTranslator = memberNameCaseStyle.HasValue || typeNameCaseStyle.HasValue
            ? new DbEnumNameTranslator(typeNameCaseStyle, memberNameCaseStyle)
            : null;
        
        Parent.Conventions.Enums.AddMapping(new DbEnumMapping(runtimeType, databaseTypeName, nameTranslator, Parent.Conventions));
        return this;
    }

    /// <summary>
    /// Adds a collection of mappings for specific enum types to database types.
    /// </summary>
    /// <param name="mappings">
    /// An array of <see cref="DbEnumMapping"/> instances to be added to the convention set.
    /// </param>
    /// <returns>
    /// A reference to the current <see cref="DbEnumConventionBuilder"/> instance, allowing for method chaining.
    /// </returns>
    public DbEnumConventionBuilder UseMappings(params DbEnumMapping[] mappings)
    {
        foreach (var mapping in mappings)
        {
            mapping.Conventions = Parent.Conventions;
            Parent.Conventions.Enums.AddMapping(mapping);
        }
        
        return this;
    }
}