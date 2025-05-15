using System.Reflection;
using Dapper;
using Flowsy.Core;

namespace Flowsy.Db.Unity.Conventions;

/// <summary>
/// A type map that uses a naming convention to match database columns to class properties.
/// </summary>
public class DbConventionTypeMap : SqlMapper.ITypeMap
{
    private readonly Dictionary<ConstructorInfo, IDictionary<string, SqlMapper.IMemberMap>> _constructorMappings = new ();
    private readonly Dictionary<string, SqlMapper.IMemberMap> _memberMappings = new ();
    
    private readonly Type _type;
    private readonly DbObjectNameConvention _columnNaming;
    
    // TODO: Add support for strict mode
    private readonly bool _strictMode;

    /// <summary>
    /// Initializes a new instance of the <see cref="DbConventionTypeMap"/> class.
    /// </summary>
    /// <param name="type">
    /// The type to map.
    /// </param>
    /// <param name="columnNaming">
    /// The naming convention to use for the columns.
    /// </param>
    /// <param name="strictMode">
    /// Whether to use strict mode. In strict mode, the type all the columns from query results must have a corresponding property in the type.
    /// </param>
    public DbConventionTypeMap(Type type, DbObjectNameConvention columnNaming, bool strictMode = false)
    {
        _type = type;
        _columnNaming = columnNaming;
        _strictMode = strictMode;
    }

    /// <summary>
    /// Finds best constructor.
    /// </summary>
    /// <param name="names">
    /// DataReader column names.
    /// </param>
    /// <param name="types">
    /// DataReader column types.
    /// </param>
    /// <returns>
    /// Matching constructor or default one.
    /// </returns>
    public ConstructorInfo? FindConstructor(string[] names, Type[] types)
        => _type
            .GetConstructors()
            .FirstOrDefault(c => IsConstructorMatch(c, names, types));

    /// <summary>
    /// Returns a constructor which should *always* be used.
    /// Parameters will be default values, nulls for reference types and zero'd for value types.
    /// Use this class to force object creation away from parameterless constructors you don't control.
    /// </summary>
    /// <returns></returns>
    public ConstructorInfo? FindExplicitConstructor()
        => _type
            .GetConstructors()
            .OrderBy(c => c.GetParameters().Length)
            .FirstOrDefault();

    /// <summary>
    /// Gets mapping for constructor parameter.
    /// </summary>
    /// <param name="constructor">
    /// Constructor to resolve.
    /// </param>
    /// <param name="columnName">
    /// DataReader column name.
    /// </param>
    /// <returns>
    /// Mapping implementation.
    /// </returns>
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
            .FirstOrDefault(p => ToColumnName(p.Name) == columnName);

        if (parameter is null)
            return null;

        memberMap = new DbBasicMemberMap(columnName, parameter.ParameterType, parameter);
        mapping[columnName] = memberMap;
        return memberMap;
    }

    /// <summary>
    /// Gets member mapping for column.
    /// </summary>
    /// <param name="columnName">
    /// DataReader column name.
    /// </param>
    /// <returns>
    /// Mapping implementation.
    /// </returns>
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

        if (field is null)
            return null;

        memberMap = new DbBasicMemberMap(columnName, field.FieldType, field);
        _memberMappings[columnName] = memberMap;
        return memberMap;
    }
    
    private string ToColumnName(string memberName)
    {
        var caseStyle = _columnNaming.CaseStyle;
        var memberNameInCaseStyle = caseStyle.HasValue && !memberName.MatchesCaseStyle(caseStyle.Value) 
            ? memberName.ApplyCaseStyle(caseStyle.Value)
            : memberName;
        var prefix = _columnNaming.Prefix ?? string.Empty;
        var suffix = _columnNaming.Suffix ?? string.Empty;
        
        return $"{prefix}{memberNameInCaseStyle}{suffix}";
    }
    
    private bool IsParameterMatch(ParameterInfo parameter, string columnName, Type columnType)
        => parameter.Name is not null &&
           columnName == ToColumnName(parameter.Name) &&
           parameter.ParameterType.IsAssignableFrom(columnType);

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
}