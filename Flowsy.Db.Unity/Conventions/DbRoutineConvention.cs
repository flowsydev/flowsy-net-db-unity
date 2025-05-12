using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Reflection;
using Dapper;
using Flowsy.Core;

namespace Flowsy.Db.Unity.Conventions;

public class DbRoutineConvention : DbConvention
{
    private static readonly ConcurrentDictionary<string, DbRoutineDescriptor> RoutineCache = new();
    
    internal DbRoutineConvention(DbConventionSet conventions) : base(conventions)
    {
        Procedures = new DbRoutineNamingConvention(conventions);
        Functions = new DbRoutineNamingConvention(conventions);
    }

    public DbRoutineType PreferredType { get; internal set; } = DbRoutineType.StoredProcedure;
    
    public DbRoutineNamingConvention Procedures { get; private set; }
    public DbRoutineNamingConvention Functions { get; private set; }
    
    public DbRoutineDescriptor BuildDescriptor(
        string routineName,
        DbRoutineType? routineType = null,
        bool? useNamedParameters = null,
        bool returnsTable = false,
        dynamic? parameters = null
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
                useNamedParameters,
                returnsTable,
                finalParameters
            ),
            finalParameters,
            transaction,
            cancellationToken
            );
    }
    
    public void CopyTo(DbRoutineConvention other)
    {
        other.PreferredType = PreferredType;
        other.Procedures = Procedures.Clone(Conventions);
        other.Functions = Functions.Clone(Conventions);
    }

    public DbRoutineConvention Clone(DbConventionSet parentConventions)
    {
        var clone = new DbRoutineConvention(parentConventions);
        CopyTo(clone);
        return clone;
    }
}