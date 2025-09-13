namespace Flowsy.Db.Unity.Conventions;

/// <summary>
/// Represents conventions for database routines (stored procedures and functions).
/// </summary>
/// <param name="RoutineType">The default routine type to use when none is explicitly specified.</param>
/// <param name="CaseStyle">The naming style to apply to routine names.</param>
/// <param name="Prefix">Optional prefix to add to routine names.</param>
/// <param name="Suffix">Optional suffix to add to routine names.</param>
public record DbRoutineConvention(
    DbRoutineType RoutineType,
    DbCaseStyle? CaseStyle = null,
    string? Prefix = null,
    string? Suffix = null
) : DbObjectConvention(CaseStyle, Prefix, Suffix)
{
    /// <summary>
    /// Default routine convention that uses stored procedures.
    /// </summary>
    public static readonly DbRoutineConvention Default = new (DbRoutineType.StoredProcedure);

    /// <summary>
    /// Prepares a routine call with the specified parameters and conventions.
    /// </summary>
    /// <param name="routineName">The name of the routine to call.</param>
    /// <param name="routineType">The type of routine. If null, uses the default routine type.</param>
    /// <param name="parameters">The parameters to pass to the routine.</param>
    /// <param name="returnsTable">Indicates whether the routine returns a table.</param>
    /// <returns>A <see cref="DbRoutineCall"/> object containing the formatted SQL statement and parameter builder.</returns>
    public DbRoutineCall PrepareCall(string routineName, DbRoutineType? routineType = null, dynamic? parameters = null, bool returnsTable = false)
    {
        var finalProvider = ConventionSet?.Provider ?? DbProviderDescriptor.Generic;
        var finalParameterConvention = ConventionSet?.Parameters ?? DbParameterConvention.Default;

        var finalRoutineName = FormatName(routineName);
        
        var parameterBuilder = new DbParameterBuilder(ConventionSet ?? DbConventionSet.Default, parameters);
        
        var statement = finalProvider.FormatRoutineCall(
            finalRoutineName,
            routineType ?? RoutineType,
            finalParameterConvention.UseNamedParameters,
            returnsTable,
            parameterBuilder.BuildDescriptors().ToArray()    
        );
        
        return new DbRoutineCall(statement, parameterBuilder);
    }
}