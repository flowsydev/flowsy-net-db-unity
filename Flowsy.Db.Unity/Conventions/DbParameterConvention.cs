using System.Collections;
using System.Data;
using System.Reflection;
using Dapper;
using Flowsy.Core;
using Flowsy.Db.Unity.Resources;

namespace Flowsy.Db.Unity.Conventions;

public class DbParameterConvention : DbConvention
{
    internal DbParameterConvention(DbConventionSet conventions) : base(conventions)
    {
    }

    public DbObjectNameConvention Naming { get; internal set; } = new();
    
    public string ResolveParameterName(string runtimeName)
    {
        var parameterNaming = Conventions.Parameters.Naming;
        
        var caseStyle = parameterNaming.CaseStyle ?? Conventions.DefaultCaseStyle;
        var nameInCaseStyle = caseStyle.HasValue && !runtimeName.MatchesCaseStyle(caseStyle.Value)
            ? runtimeName.ApplyCaseStyle(caseStyle.Value)
            : runtimeName;
        var prefix = parameterNaming.Prefix ?? string.Empty;
        var suffix = parameterNaming.Suffix ?? string.Empty;
     
        return $"{prefix}{nameInCaseStyle}{suffix}";
    }

    public IEnumerable<DbParameterDescriptor> BuildDescriptors(IEnumerable<PropertyInfo> properties)
        => (from p in properties select BuildDescriptor(p.Name, p.PropertyType)).ToArray();
    
    public IEnumerable<DbParameterDescriptor> BuildDescriptors(dynamic? parameters = null)
    {
        if (parameters is IDictionary<string, object?> dictionary)
        {
            return (
                from entry in dictionary
                select BuildDescriptor(entry)
            ).ToArray();
        }
        
        object param = parameters ?? new { };
        var paramType = param.GetType();
        var paramProperties = paramType.GetRuntimeProperties();

        return (
            from p in paramProperties
            select BuildDescriptor(p.Name, p.PropertyType)
        ).ToArray();
    }

    public DbParameterDescriptor BuildDescriptor(KeyValuePair<string, object?> keyValuePair)
    {
        var (key, value) = keyValuePair;
        return BuildDescriptor(key, value?.GetType() ?? typeof(object));
    }
    
    public DbParameterDescriptor BuildDescriptor(string runtimeName, Type runtimeType)
    {
        var provider = Conventions.Provider;
        var parameterName = ResolveParameterName(runtimeName);

        if (runtimeType is not {IsArray: true, HasElementType: true} || !provider.SupportsArrays)
            return runtimeType.IsEnum
                ? BuildDescriptorForEnum(parameterName, runtimeType)
                : new DbParameterDescriptor(
                    provider,
                    parameterName,
                    runtimeType,
                    provider.GetDatabaseType(runtimeType)
                );
        
        var elementType = runtimeType.GetElementType();
        if (elementType is not null)
        {
            return elementType.IsEnum 
                ? BuildDescriptorForEnum(parameterName, elementType, true)
                : new DbParameterDescriptor(provider, parameterName, runtimeType, provider.GetDatabaseType(elementType));
        }

        return runtimeType.IsEnum 
            ? BuildDescriptor(parameterName, runtimeType) 
            : new DbParameterDescriptor(
                provider,
                parameterName,
                runtimeType,
                provider.GetDatabaseType(runtimeType)
            );
    }

    public DbParameterDescriptor BuildDescriptorForEnum(string parameterName, Type runtimeType, bool asArray = false)
    {
        if (!runtimeType.IsEnum)
            throw new ArgumentException(Strings.TypeMustBeAnEnum, nameof(runtimeType));
        
        Conventions.Enums.Map(runtimeType, out var enumDatabaseType, out var customType, out _);

        var provider = Conventions.Provider;
        var providerSupportsEnums = provider.SupportsEnumsAsCustomTypes;
        
        return new DbParameterDescriptor(
            provider,
            parameterName,
            runtimeType,
            enumDatabaseType,
            providerSupportsEnums ? asArray ? provider.FormatArrayType(customType) : customType : null,
            providerSupportsEnums && !string.IsNullOrEmpty(customType) 
                ? DbValueExpression.CustomTypeCast 
                : DbValueExpression.Raw
            );
    }

    public DynamicParameters BuildDynamicParameters(dynamic? parameters = null)
    {
        object param = parameters ?? new { };
        var properties = param.GetType().GetRuntimeProperties();
        var dynamicParameters = new DynamicParameters();
        foreach (var property in properties)
        {
            var descriptor = BuildDescriptor(property.Name, property.PropertyType);
            
            dynamicParameters.Add(
                descriptor.Name,
                descriptor.ResolveDatabaseValue(property.GetValue(param), Conventions),
                descriptor.DatabaseType, 
                descriptor.Direction, 
                descriptor.Size,
                descriptor.Precision,
                descriptor.Scale
            );
        }
        return dynamicParameters;
    }
    
    public DynamicParameters BuildDynamicParameters(IEnumerable<DbParameterDescriptor> parameterDescriptors, dynamic? parameters = null)
    {
        var dictionary = new Dictionary<string, object?>();
        object finalParameters = parameters ?? new { };
        switch (finalParameters)
        {
            case IDictionary<string, object?> d:
                foreach (var (k, v) in d) dictionary[ResolveParameterName(k)] = v;
                break;
            case IEnumerable<KeyValuePair<string, object?>> enumerable:
                dictionary = enumerable.ToDictionary(kvp => ResolveParameterName(kvp.Key), kvp => kvp.Value);
                break;
            default:
            {
                var finalParametersType = finalParameters.GetType();
                var finalParametersProperties = finalParametersType.GetRuntimeProperties();
                foreach (var property in finalParametersProperties)
                    dictionary[ResolveParameterName(property.Name)] = property.GetValue(finalParameters);
                
                break;
            }
        }
        
        var dynamicParameters = new DynamicParameters();
        foreach (var descriptor in parameterDescriptors)
        {
            dynamicParameters.Add(
                descriptor.Name,
                descriptor.ResolveDatabaseValue(dictionary.GetValueOrDefault(descriptor.Name), Conventions),
                descriptor.DatabaseType, 
                descriptor.Direction, 
                descriptor.Size,
                descriptor.Precision,
                descriptor.Scale
                );
        }
        return dynamicParameters;
    }
    
    public void CopyTo(DbParameterConvention other)
    {
        other.Naming = Naming;
    }

    public DbParameterConvention Clone(DbConventionSet parentConventions)
    {
        var clone = new DbParameterConvention(parentConventions);
        CopyTo(clone);
        return clone;
    }
}