using System.Reflection;
using Dapper;
using Flowsy.Db.Unity.Resources;

namespace Flowsy.Db.Unity.Conventions;

/// <summary>
/// Represents a type mapping for database objects that applies naming conventions, implementing Dapper's ITypeMap interface.
/// </summary>
public class DbConventionTypeMap : SqlMapper.ITypeMap
{
    private readonly Dictionary<ConstructorInfo, IDictionary<string, SqlMapper.IMemberMap>> _constructorMappings = new ();
    private readonly Dictionary<string, SqlMapper.IMemberMap> _memberMappings = new ();
    
    private readonly Type _type;
    private readonly DbObjectConvention _columnConvention;
    private readonly bool _strictMode;

    /// <summary>
    /// Initializes a new instance of the <see cref="DbConventionTypeMap"/> class.
    /// </summary>
    /// <param name="type">The type to map.</param>
    /// <param name="columnConvention">The naming convention to apply to column names.</param>
    /// <param name="strictMode">If true, throws exceptions when mappings cannot be found.</param>
    public DbConventionTypeMap(Type type, DbObjectConvention columnConvention, bool strictMode = false)
    {
        _type = type;
        _columnConvention = columnConvention;
        _strictMode = strictMode;
    }

    /// <summary>
    /// Finds a constructor that matches the specified parameter names and types.
    /// </summary>
    /// <param name="names">The parameter names to match.</param>
    /// <param name="types">The parameter types to match.</param>
    /// <returns>A matching constructor, or null if none is found.</returns>
    public ConstructorInfo? FindConstructor(string[] names, Type[] types)
    {
        var constructor = _type
            .GetConstructors()
            .FirstOrDefault(c => IsConstructorMatch(c, names, types));
        
        if (constructor is not null)
            return constructor;
        
        if (_strictMode)
            throw new InvalidOperationException(string.Format(Strings.NoMatchingConstructorFoundForTypeX, _type.Name));

        return null;
    }

    /// <summary>
    /// Finds an explicit constructor for the type.
    /// </summary>
    /// <returns>A constructor, or null if none is found.</returns>
    public ConstructorInfo? FindExplicitConstructor()
    {
        var constructor = _type
            .GetConstructors()
            .OrderBy(c => c.GetParameters().Length)
            .FirstOrDefault();
        
        if (constructor is not null)
            return constructor;
        
        if (_strictMode)
            throw new InvalidOperationException(string.Format(Strings.NoMatchingConstructorFoundForTypeX, _type.Name));
        
        return null;
    }

    /// <summary>
    /// Gets the member map for a constructor parameter with the specified column name.
    /// </summary>
    /// <param name="constructor">The constructor to search for parameters.</param>
    /// <param name="columnName">The name of the database column to map.</param>
    /// <returns>A member map for the constructor parameter, or null if not found.</returns>
    public SqlMapper.IMemberMap? GetConstructorParameter(ConstructorInfo constructor, string columnName)
    {
        if (!_constructorMappings.TryGetValue(constructor, out var mapping))
        {
            mapping = new Dictionary<string, SqlMapper.IMemberMap>();
            _constructorMappings[constructor] = mapping;
        }

        if (mapping.TryGetValue(columnName, out var memberMap))
            return memberMap;

        var parameter = constructor
            .GetParameters()
            .FirstOrDefault(p => !string.IsNullOrEmpty(p.Name) && ToColumnName(p.Name) == columnName);

        if (parameter is null)
            return null;

        memberMap = new DbBasicMemberMap(columnName, parameter.ParameterType, parameter);
        mapping[columnName] = memberMap;
        return memberMap;
    }

    /// <summary>
    /// Gets the member map for a property or field with the specified column name.
    /// </summary>
    /// <param name="columnName">The name of the database column to map.</param>
    /// <returns>A member map for the member, or null if not found.</returns>
    public SqlMapper.IMemberMap? GetMember(string columnName)
    {
        if (_memberMappings.TryGetValue(columnName, out var memberMap))
            return memberMap;

        var property = _type
            .GetRuntimeProperties()
            .FirstOrDefault(p => ToColumnName(p.Name) == columnName);

        if (property is not null)
        {
            memberMap = new DbBasicMemberMap(columnName, property.PropertyType, property);
            _memberMappings[columnName] = memberMap;
            return memberMap;
        }
        
        var field = _type
            .GetRuntimeFields()
            .FirstOrDefault(f => ToColumnName(f.Name) == columnName);

        if (field is not null)
        {
            memberMap = new DbBasicMemberMap(columnName, field.FieldType, field);
            _memberMappings[columnName] = memberMap;
            return memberMap;
        }

        if (_strictMode)
            throw new InvalidOperationException(string.Format(Strings.NoMemberFoundForColumnXInTypeY, columnName, _type.Name));
        
        return null;
    }
    
    /// <summary>
    /// Converts a member name to the corresponding database column name using the configured naming conventions.
    /// </summary>
    /// <param name="memberName">The name of the .NET member (property, field, or parameter).</param>
    /// <returns>The formatted database column name.</returns>
    private string ToColumnName(string memberName)
    {
        var caseStyle = _columnConvention.CaseStyle;
        var memberNameInCaseStyle = caseStyle.HasValue && caseStyle.Value != DbCaseStyle.None 
            ? caseStyle.Value.Apply(memberName)
            : memberName;
        var prefix = _columnConvention.Prefix ?? string.Empty;
        var suffix = _columnConvention.Suffix ?? string.Empty;
        
        return $"{prefix}{memberNameInCaseStyle}{suffix}";
    }
    
    /// <summary>
    /// Determines if a constructor parameter matches the specified column name and type.
    /// </summary>
    /// <param name="parameter">The parameter to check.</param>
    /// <param name="columnName">The database column name.</param>
    /// <param name="columnType">The database column type.</param>
    /// <returns>True if the parameter matches; otherwise, false.</returns>
    private bool IsParameterMatch(ParameterInfo parameter, string columnName, Type columnType)
        => parameter.Name is not null &&
           columnName == ToColumnName(parameter.Name) &&
           parameter.ParameterType.IsAssignableFrom(columnType);

    /// <summary>
    /// Determines if a constructor matches the specified parameter names and types.
    /// </summary>
    /// <param name="constructor">The constructor to check.</param>
    /// <param name="names">The parameter names to match.</param>
    /// <param name="types">The parameter types to match.</param>
    /// <returns>True if the constructor matches; otherwise, false.</returns>
    private bool IsConstructorMatch(ConstructorInfo constructor, string[] names, Type[] types)
    {
        var parameters = constructor.GetParameters();
        if (parameters.Length != names.Length && parameters.Length != types.Length)
            return false;
        
        for (var index = 0; index <= parameters.Length; index++)
            if (!IsParameterMatch(parameters[index], names[index], types[index]))
                return false;

        return true;
    }

    /// <summary>
    /// Registers the type mapping for the specified target type.
    /// </summary>
    /// <param name="targetType">The target type to register.</param>
    /// <param name="columnConvention">The naming convention to apply to column names.</param>
    /// <param name="strictMode">If true, throws exceptions when mappings cannot be found.</param>
    public static void Register(Type targetType, DbObjectConvention columnConvention, bool strictMode = false)
    {
        SqlMapper.RemoveTypeMap(targetType);
        SqlMapper.SetTypeMap(targetType, new DbConventionTypeMap(targetType, columnConvention, strictMode));
    }
    
    /// <summary>
    /// Registers the type mapping for the specified target type with the given case style.
    /// </summary>
    /// <param name="targetType">The target type to register.</param>
    /// <param name="caseStyle">The case style to apply to column names.</param>
    /// <param name="strictMode">If true, throws exceptions when mappings cannot be found.</param>
    public static void Register(Type targetType, DbCaseStyle? caseStyle, bool strictMode = false)
        => Register(targetType, new DbObjectConvention(caseStyle, null, null), strictMode);

    /// <summary>
    /// Registers the type mapping for the specified target type with the given case style, prefix, and suffix.
    /// </summary>
    /// <param name="targetType">The target type to register.</param>
    /// <param name="caseStyle">The case style to apply to column names.</param>
    /// <param name="prefix">The prefix to apply to column names.</param>
    /// <param name="suffix">The suffix to apply to column names.</param>
    /// <param name="strictMode">If true, throws exceptions when mappings cannot be found.</param>
    public static void Register(Type targetType, DbCaseStyle? caseStyle, string? prefix = null, string? suffix = null, bool strictMode = false)
        => Register(targetType, new DbObjectConvention(caseStyle, prefix, suffix), strictMode);
}