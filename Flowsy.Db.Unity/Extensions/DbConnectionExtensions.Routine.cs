using System.Data;
using Dapper;
using Flowsy.Db.Unity.Conventions;

namespace Flowsy.Db.Unity.Extensions;

public static partial class DbConnectionExtensions
{
    private static CommandDefinition BuildCommandDefinition(
        string routineName,
        DbRoutineType? routineType = null,
        dynamic? parameters = null, 
        bool returnsTable = false,
        IDbTransaction? transaction = null,
        DbConventionSet? conventions = null,
        CancellationToken cancellationToken = default
    )
    {
        var finalConventions = conventions ?? DbConventionSet.Default;
        var finalRoutineType = routineType ?? finalConventions.Routines.PreferredType;
        var useNamedParameters = finalConventions.Provider.SupportsNamedParameters && (
            finalRoutineType == DbRoutineType.StoredProcedure
                ? finalConventions.Routines.Procedures.UseNamedParameters
                : finalConventions.Routines.Functions.UseNamedParameters
        );
        return finalConventions.Routines.BuildCommandDefinition(
            routineName,
            finalRoutineType,
            parameters as object,
            useNamedParameters,
            returnsTable,
            transaction,
            cancellationToken
            );
    }
}