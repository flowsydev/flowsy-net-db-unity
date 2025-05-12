using System.Reflection;
using Dapper;
using Flowsy.Core;

namespace Flowsy.Db.Unity.Conventions;

public class DbConventionTypeMap : SqlMapper.ITypeMap
{
    private readonly Dictionary<ConstructorInfo, IDictionary<string, SqlMapper.IMemberMap>> _constructorMappings = new ();
    private readonly Dictionary<string, SqlMapper.IMemberMap> _memberMappings = new ();
    
    private readonly Type _type;
    private readonly DbObjectNameConvention _columnNaming;
    
    // TODO: Add support for strict mode
    private readonly bool _strictMode;

    public DbConventionTypeMap(Type type, DbObjectNameConvention columnNaming, bool strictMode = false)
    {
        _type = type;
        _columnNaming = columnNaming;
        _strictMode = strictMode;
    }

    public ConstructorInfo? FindConstructor(string[] names, Type[] types)
        => _type
            .GetConstructors()
            .FirstOrDefault(c => IsConstructorMatch(c, names, types));

    public ConstructorInfo? FindExplicitConstructor()
        => _type
            .GetConstructors()
            .OrderBy(c => c.GetParameters().Length)
            .FirstOrDefault();

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