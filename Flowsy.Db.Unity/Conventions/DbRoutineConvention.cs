using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Reflection;
using Dapper;
using Flowsy.Core;

namespace Flowsy.Db.Unity.Conventions;

/// <summary>
/// Builder for configuring routine conventions.
/// </summary>
public class DbRoutineConvention : DbConvention
{
    private static readonly ConcurrentDictionary<string, DbRoutineDescriptor> RoutineCache = new();
    
    internal DbRoutineConvention(DbConventionSet conventions) : base(conventions)
    {
        Procedures = new DbRoutineNamingConvention(conventions);
        Functions = new DbRoutineNamingConvention(conventions);
    }

    /// <summary>
    /// The preferred type to use when invoking database routines. 
    /// </summary>
    public DbRoutineType PreferredType { get; internal set; } = DbRoutineType.StoredProcedure;
    
    /// <summary>
    /// The naming convention for stored procedures.
    /// </summary>
    public DbRoutineNamingConvention Procedures { get; private set; }
    
    /// <summary>
    /// The naming convention for stored functions.
    /// </summary>
    public DbRoutineNamingConvention Functions { get; private set; }
    
    /// <summary>
    /// Builds a descriptor for a database routine based on the provided parameters.
    /// </summary>
    /// <param name="routineName">
    /// The name of the routine.
    /// This can be a fully qualified name or a simple name.
    /// If a fully qualified name is provided, it will be parsed using the naming convention. The prefix and suffix will be applied to the last part of the name.
    /// </param>
    /// <param name="routineType">
    /// The type of the routine (stored procedure or function).
    /// </param>
    /// <param name="parameters">
    /// The parameters to be passed to the routine. This can be an anonymous object or a dictionary.
    /// </param>
    /// <param name="useNamedParameters">
    /// Whether to use named parameters when invoking the routine.
    /// Named parameters will be used only if supported by the underlying database provider.
    /// </param>
    /// <param name="returnsTable">
    /// Whether the routine returns a table. A scalar value is assumed if this is false.
    /// </param>
    /// <returns>
    /// A <see cref="DbRoutineDescriptor"/> representing the routine.
    /// </returns>
    public DbRoutineDescriptor BuildDescriptor(
        string routineName,
        DbRoutineType? routineType = null,
        dynamic? parameters = null,
        bool? useNamedParameters = null,
        bool returnsTable = false
        )
    {
        var finalRoutineType = routineType ?? PreferredType;
        var routineConvention = finalRoutineType == DbRoutineType.StoredProcedure
            ? Procedures
            : Functions;

        var nameConvention = routineConvention.Naming;
        var caseStyle = nameConvention.CaseStyle ?? Conventions.DefaultCaseStyle;
        var prefix = nameConvention.Prefix ?? string.Empty;
        var suffix = nameConvention.Suffix ?? string.Empty;
        var nameConventionKey = $"{caseStyle}:{prefix}:{suffix}";

        object finalParameters = parameters ?? new { };
        var parameterProperties = finalParameters.GetType().GetRuntimeProperties().ToArray();
        var parameterKey = string.Join(",", parameterProperties.Select(p => p.Name));
        
        var routineKey = $"{finalRoutineType}:{nameConventionKey}:{routineName}:{parameterKey}";

        if (RoutineCache.TryGetValue(routineKey, out var routineDescriptor)) 
            return routineDescriptor;
        
        Func<string, int, int, string>? transform = caseStyle.HasValue
            ? (part, index, length) =>
            {
                var simpleName = !part.MatchesCaseStyle(caseStyle.Value) ? part.ApplyCaseStyle(caseStyle.Value) : part;
                return index < length - 1 ? simpleName : $"{prefix}{simpleName}{suffix}";
            }
            : null;  
        var fullyQualifiedName = Conventions.Provider.ParseObjectName(routineName, transform);
        var parameterDescriptors = Conventions.Parameters.BuildDescriptors(parameterProperties);
        
        routineDescriptor = new DbRoutineDescriptor(
            fullyQualifiedName, 
            routineType ?? PreferredType,
            useNamedParameters ?? routineConvention.UseNamedParameters,
            returnsTable,
            parameterDescriptors
        );
            
        RoutineCache.TryAdd(routineKey, routineDescriptor);

        return routineDescriptor;
    }

    /// <summary>
    /// Builds a command definition for a database routine based on the provided parameters.
    /// </summary>
    /// <param name="routineName">
    /// The name of the routine.
    /// This can be a fully qualified name or a simple name.
    /// If a fully qualified name is provided, it will be parsed using the naming convention. The prefix and suffix will be applied to the last part of the name.
    /// </param>
    /// <param name="routineType">
    /// The type of the routine (stored procedure or function).
    /// </param>
    /// <param name="parameters">
    /// The parameters to be passed to the routine. This can be an anonymous object or a dictionary.
    /// </param>
    /// <param name="useNamedParameters">
    /// Whether to use named parameters when invoking the routine.
    /// Named parameters will be used only if supported by the underlying database provider.
    /// </param>
    /// <param name="returnsTable">
    /// Whether the routine returns a table. A scalar value is assumed if this is false.
    /// </param>
    /// <param name="transaction">
    /// An optional transaction to use when executing the command.
    /// </param>
    /// <param name="cancellationToken">
    /// An optional cancellation token for the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="CommandDefinition"/> representing the command to be executed
    /// </returns>
    public CommandDefinition BuildCommandDefinition(
        string routineName,
        DbRoutineType? routineType = null,
        dynamic? parameters = null,
        bool? useNamedParameters = null,
        bool returnsTable = false,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default
        )
    {
        object? finalParameters = parameters;
        return Conventions.Commands.BuildDefinition(
            BuildDescriptor(
                routineName,
                routineType,
                finalParameters,
                useNamedParameters,
                returnsTable
            ),
            finalParameters,
            transaction,
            cancellationToken
            );
    }
    
    /// <summary>
    /// Copies the properties of this instance to another instance of <see cref="DbRoutineConvention"/>.
    /// </summary>
    /// <param name="other">
    /// The <see cref="DbRoutineConvention"/> instance to copy properties to.
    /// </param>
    public void CopyTo(DbRoutineConvention other)
    {
        other.PreferredType = PreferredType;
        other.Procedures = Procedures.Clone(Conventions);
        other.Functions = Functions.Clone(Conventions);
    }

    /// <summary>
    /// Creates a clone of this <see cref="DbRoutineConvention"/> instance.
    /// </summary>
    /// <param name="parentConventions">
    /// The parent <see cref="DbConventionSet"/> to which the cloned instance will belong.
    /// </param>
    /// <returns>
    /// A new instance of <see cref="DbRoutineConvention"/> with the same properties as this instance.
    /// </returns>
    public DbRoutineConvention Clone(DbConventionSet parentConventions)
    {
        var clone = new DbRoutineConvention(parentConventions);
        CopyTo(clone);
        return clone;
    }
}