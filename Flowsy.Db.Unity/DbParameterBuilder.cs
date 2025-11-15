using System.Data;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Dapper;
using Flowsy.Db.Unity.Conventions;
using Flowsy.Db.Unity.Resources;
using Microsoft.CSharp.RuntimeBinder;
using Binder = Microsoft.CSharp.RuntimeBinder.Binder;

namespace Flowsy.Db.Unity;

/// <summary>
/// Builds a set of database parameters from a dynamic object.
/// </summary>
public class DbParameterBuilder
{
    private readonly DbConventionSet _conventions;
    private readonly dynamic? _parameters;
    private IList<DbParameterDescriptor> _descriptors = [];
    private DynamicParameters _dynamicParameters = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="DbParameterBuilder"/> class.
    /// </summary>
    /// <param name="conventions">
    /// The convention set to use for building the parameters.
    /// </param>
    /// <param name="parameters">
    /// The dynamic object containing the parameters to build.
    /// </param>
    public DbParameterBuilder(DbConventionSet conventions, dynamic? parameters)
    {
        _conventions = conventions;
        _parameters = parameters;
    }
    
    /// <summary>
    /// Builds and returns a list of database parameter descriptors.
    /// </summary>
    /// <returns>
    /// A list of database parameter descriptors.
    /// </returns>
    public IEnumerable<DbParameterDescriptor> BuildDescriptors()
    {
        if (_descriptors.Count == 0)
            Build(_parameters);
        
        return _descriptors;
    }
    
    /// <summary>
    /// Builds and returns a <see cref="DynamicParameters"/> object with the database parameters.
    /// </summary>
    /// <returns>
    /// A <see cref="DynamicParameters"/> object with the database parameters.
    /// </returns>
    public DynamicParameters BuildDynamicParameters()
    {
        if (!_dynamicParameters.ParameterNames.Any())
            Build(_parameters);
        
        return _dynamicParameters;
    }

    private void Build(dynamic? parameters)
    {
        _descriptors = [];
        _dynamicParameters = new DynamicParameters();
        
        if (parameters is null)
            return;

        var properties = GetProperties((object) parameters);
        foreach (var (propertyName, propertyType, propertyValue) in properties)
        {
            var descriptor = BuildDescriptor(propertyName, propertyType);
            var databaseValue = descriptor.ResolveDatabaseValue(propertyValue, _conventions);
            
            _dynamicParameters.Add(
                descriptor.Name,
                databaseValue,
                descriptor.DatabaseType,
                descriptor.Direction ?? ParameterDirection.Input, 
                descriptor.Size,
                descriptor.Precision,
                descriptor.Scale
                );
            
            _descriptors.Add(descriptor);
        }
    }
    
    /// <summary>
    /// Builds a parameter descriptor for the specified runtime name and type.
    /// </summary>
    /// <param name="runtimeName">
    /// The runtime name of the parameter.
    /// </param>
    /// <param name="runtimeType">
    /// The runtime type of the parameter.
    /// </param>
    /// <returns>
    /// A <see cref="DbParameterDescriptor"/> configured for the specified parameter.
    /// </returns>
    public DbParameterDescriptor BuildDescriptor(string runtimeName, Type runtimeType)
    {
        var provider = _conventions.Provider;
        var parameterName = _conventions.Parameters.FormatName(runtimeName);

        if (runtimeType is not {IsArray: true, HasElementType: true} || !provider.SupportsArrays)
        {
            if (!runtimeType.IsGenericType || runtimeType.GetGenericTypeDefinition() != typeof(Nullable<>))
                return runtimeType.IsEnum
                    ? BuildDescriptorForEnum(parameterName, runtimeType)
                    : new DbParameterDescriptor(
                        provider,
                        parameterName,
                        runtimeType,
                        provider.InferDatabaseType(runtimeType)
                    );
            
            var underlyingType = Nullable.GetUnderlyingType(runtimeType)!;
            return underlyingType.IsEnum 
                ? BuildDescriptorForEnum(parameterName, underlyingType)
                : new DbParameterDescriptor(
                    provider,
                    parameterName,
                    underlyingType,
                    provider.InferDatabaseType(underlyingType)
                );
        }
        
        var elementType = runtimeType.GetElementType();
        if (elementType is not null)
        {
            // Array de enums
            if (elementType.IsEnum)
                return BuildDescriptorForEnum(parameterName, elementType, true);
            
            // Array de Nullable<T>
            if (elementType.IsGenericType && elementType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var underlyingType = Nullable.GetUnderlyingType(elementType)!;
                return underlyingType.IsEnum
                    ? BuildDescriptorForEnum(parameterName, underlyingType, true)
                    : new DbParameterDescriptor(
                        provider,
                        parameterName,
                        runtimeType,
                        null  // No especificar DbType para arrays, dejar que el proveedor lo infiera
                    );
            }
            
            // Array de tipos primitivos u otros tipos
            return new DbParameterDescriptor(
                provider,
                parameterName,
                runtimeType,
                null  // No especificar DbType para arrays, dejar que el proveedor lo infiera
            );
        }

        return runtimeType.IsEnum 
            ? BuildDescriptorForEnum(parameterName, runtimeType) 
            : new DbParameterDescriptor(
                provider,
                parameterName,
                runtimeType,
                provider.InferDatabaseType(runtimeType)
            );
    }
    
    /// <summary>
    /// Builds a parameter descriptor for an enum type.
    /// </summary>
    /// <param name="parameterName">
    /// The name of the parameter.
    /// </param>
    /// <param name="runtimeType">
    /// The enum type of the parameter.
    /// </param>
    /// <param name="asArray">
    /// Indicates whether the parameter should be treated as an array.
    /// </param>
    /// <returns>
    /// A <see cref="DbParameterDescriptor"/> configured for the enum parameter.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the specified type is not an enum.
    /// </exception>
    private DbParameterDescriptor BuildDescriptorForEnum(string parameterName, Type runtimeType, bool asArray = false)
    {
        if (!runtimeType.IsEnum)
            throw new ArgumentException(Strings.TypeMustBeAnEnum, nameof(runtimeType));
        
        _conventions.Enums.Map(runtimeType, out var enumDatabaseType, out var customType, out _);

        var provider = _conventions.Provider;
        var providerSupportsEnums = provider.EnumSupport == DbEnumSupport.CustomType;
        
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
    
    private static IEnumerable<(string Name, Type Type, object? Value)> GetProperties(object? parameters)
    {
        if (parameters is null) yield break;

        // 1) ExpandoObject (common "dynamic" dictionary)
        if (parameters is IDictionary<string, object?> expando)
        {
            foreach (var kv in expando)
                yield return (kv.Key, kv.Value?.GetType() ?? typeof(object), kv.Value);
            yield break;
        }

        // 2) Other dynamic providers (e.g., DynamicObject subclasses, proxies)
        if (parameters is IDynamicMetaObjectProvider d)
        {
            var meta = d.GetMetaObject(Expression.Constant(d));
            foreach (var name in meta.GetDynamicMemberNames().Distinct())
            {
                if (TryGetDynamicMember(d, name, out var value))
                    yield return (name, value?.GetType() ?? typeof(object), value);
            }
            // Don’t return; also fall back to reflection to catch any real CLR props
            // that the dynamic object might expose.
        }

        // 3) Plain CLR types (classes, records, anonymous types)
        var t = parameters.GetType();
        var props = t.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                     .Where(p => p.CanRead && p.GetIndexParameters().Length == 0);

        foreach (var p in props)
        {
            object? value = null;
            try { value = p.GetValue(parameters); } catch { /* ignore getters that throw */ }
            yield return (p.Name, p.PropertyType, value);
        }
    }

    // Uses the C# runtime binder to read a dynamic member by name.
    private static bool TryGetDynamicMember(IDynamicMetaObjectProvider obj, string name, out object? value)
    {
        var binder = Binder.GetMember(
            CSharpBinderFlags.None,
            name,
            obj.GetType(),
            [CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)]
            );

        var site = CallSite<Func<CallSite, object, object>>.Create(binder);

        try
        {
            value = site.Target(site, obj);
            return true;
        }
        catch (RuntimeBinderException)
        {
            value = null;
            return false;
        }
    }
}