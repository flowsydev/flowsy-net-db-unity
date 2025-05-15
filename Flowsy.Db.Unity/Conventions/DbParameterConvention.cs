using System.Collections;
using System.Data;
using System.Reflection;
using Dapper;
using Flowsy.Core;
using Flowsy.Db.Unity.Resources;

namespace Flowsy.Db.Unity.Conventions;

/// <summary>
/// Represents a convention for database parameters.
/// </summary>
public class DbParameterConvention : DbConvention
{
    internal DbParameterConvention(DbConventionSet conventions) : base(conventions)
    {
    }

    /// <summary>
    /// The naming convention for database parameters.
    /// </summary>
    public DbObjectNameConvention Naming { get; internal set; } = new();
    
    /// <summary>
    /// Resolves the parameter name based on the runtime name and the naming convention.
    /// </summary>
    /// <param name="runtimeName">
    /// The runtime name of the parameter.
    /// </param>
    /// <returns>
    /// The resolved parameter name as must be passed to the database.
    /// </returns>
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

    /// <summary>
    /// Builds a collection of <see cref="DbParameterDescriptor"/> instances for the specified properties.
    /// </summary>
    /// <param name="properties">
    /// The properties to be used for building the descriptors.
    /// The property names and types will be used to resolve the parameter names and database types.
    /// </param>
    /// <returns>
    /// A collection of <see cref="DbParameterDescriptor"/> built from the specified properties.
    /// These descriptors can be used to create database parameters passed to commands.
    /// </returns>
    public IEnumerable<DbParameterDescriptor> BuildDescriptors(IEnumerable<PropertyInfo> properties)
        => (from p in properties select BuildDescriptor(p.Name, p.PropertyType)).ToArray();
    
    /// <summary>
    /// Builds a collection of <see cref="DbParameterDescriptor"/> instances for the specified parameters.
    /// </summary>
    /// <param name="parameters">
    /// An object holding properties to be used for building parameter descriptors. This can be an anonymous object or a dictionary.
    /// For anonymous objects, their properties (names and types) will be used to resolve the parameter names and database types.
    /// For dictionaries, the keys will be used as parameter names and the values to resolve the database types.
    /// </param>
    /// <returns>
    /// A collection of <see cref="DbParameterDescriptor"/> built from the specified parameters.
    /// </returns>
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

    /// <summary>
    /// Builds a <see cref="DbParameterDescriptor"/> for the specified key-value pair.
    /// </summary>
    /// <param name="keyValuePair">
    /// The key-value pair representing the parameter name and its value.
    /// </param>
    /// <returns>
    /// A <see cref="DbParameterDescriptor"/> built from the specified key-value pair.
    /// </returns>
    public DbParameterDescriptor BuildDescriptor(KeyValuePair<string, object?> keyValuePair)
    {
        var (key, value) = keyValuePair;
        return BuildDescriptor(key, value?.GetType() ?? typeof(object));
    }
    
    /// <summary>
    /// Builds a <see cref="DbParameterDescriptor"/> for the specified runtime name and type.
    /// </summary>
    /// <param name="runtimeName">
    /// The runtime name of the parameter.
    /// </param>
    /// <param name="runtimeType">
    /// The runtime type of the parameter.
    /// </param>
    /// <returns>
    /// A <see cref="DbParameterDescriptor"/> built from the specified runtime name and type.
    /// </returns>
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
    
    private DbParameterDescriptor BuildDescriptorForEnum(string parameterName, Type runtimeType, bool asArray = false)
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

    /// <summary>
    /// Builds a <see cref="DynamicParameters"/> instance for the specified parameters.
    /// The names of the parameters will be resolved using the naming convention.
    /// </summary>
    /// <param name="parameters">
    /// The parameters to be passed to the command. This can be an anonymous object or a dictionary.
    /// </param>
    /// <returns>
    /// A <see cref="DynamicParameters"/> instance that encapsulates the parameters and their values.
    /// </returns>
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
    
    /// <summary>
    /// Builds a <see cref="DynamicParameters"/> instance for the specified parameter descriptors using the provided object to resolve the values.
    /// The names of the parameters will be resolved using the naming convention.
    /// </summary>
    /// <param name="parameterDescriptors">
    /// The parameter descriptors to be used for building the dynamic parameters.
    /// </param>
    /// <param name="parameters">
    /// The parameters to be passed to the command. This can be an anonymous object or a dictionary.
    /// </param>
    /// <returns>
    /// A <see cref="DynamicParameters"/> instance that encapsulates the parameters and their values.
    /// </returns>
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
    
    /// <summary>
    /// Copies the properties of this instance to another <see cref="DbParameterConvention"/> instance.
    /// </summary>
    /// <param name="other">
    /// The other <see cref="DbParameterConvention"/> instance to copy properties to.
    /// </param>
    public void CopyTo(DbParameterConvention other)
    {
        other.Naming = Naming;
    }

    /// <summary>
    /// Creates a clone of this <see cref="DbParameterConvention"/> instance.
    /// </summary>
    /// <param name="parentConventions">
    /// The parent <see cref="DbConventionSet"/> instance to which the cloned convention will belong.
    /// </param>
    /// <returns>
    /// A new <see cref="DbParameterConvention"/> instance that is a clone of this instance.
    /// </returns>
    public DbParameterConvention Clone(DbConventionSet parentConventions)
    {
        var clone = new DbParameterConvention(parentConventions);
        CopyTo(clone);
        return clone;
    }
}