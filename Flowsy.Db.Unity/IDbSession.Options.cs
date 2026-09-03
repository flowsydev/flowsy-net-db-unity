namespace Flowsy.Db.Unity;

public partial interface IDbSession
{
    /// <summary>Executes SQL with options scoped to this call.</summary>
    Task<int> ExecuteAsync(string statement, dynamic? parameters, DbSessionCallOptions options, CancellationToken cancellationToken = default);

    /// <summary>Executes a routine with options scoped to this call.</summary>
    Task<int> ExecuteRoutineAsync(string routineName, DbRoutineType routineType, dynamic? parameters, DbSessionCallOptions options, CancellationToken cancellationToken = default);

    /// <summary>Queries multiple results with options scoped to this call.</summary>
    Task<IEnumerable<T>> QueryAsync<T>(string statement, dynamic? parameters, DbSessionCallOptions options, CancellationToken cancellationToken = default);

    /// <summary>Queries a routine with options scoped to this call.</summary>
    Task<IEnumerable<T>> QueryFromRoutineAsync<T>(string routineName, DbRoutineType routineType, dynamic? parameters, DbSessionCallOptions options, CancellationToken cancellationToken = default);

    /// <summary>Gets the first result with per-call options.</summary>
    Task<T> QueryFirstAsync<T>(string statement, dynamic? parameters, DbSessionCallOptions options, CancellationToken cancellationToken = default);

    /// <summary>Gets the first result or its default value with per-call options.</summary>
    Task<T?> QueryFirstOrDefaultAsync<T>(string statement, dynamic? parameters, DbSessionCallOptions options, CancellationToken cancellationToken = default);

    /// <summary>Gets exactly one result with per-call options.</summary>
    Task<T> QuerySingleAsync<T>(string statement, dynamic? parameters, DbSessionCallOptions options, CancellationToken cancellationToken = default);

    /// <summary>Gets exactly one result or its default value with per-call options.</summary>
    Task<T?> QuerySingleOrDefaultAsync<T>(string statement, dynamic? parameters, DbSessionCallOptions options, CancellationToken cancellationToken = default);

    /// <summary>Gets the first routine result with per-call options.</summary>
    Task<T> QueryFirstFromRoutineAsync<T>(string routineName, DbRoutineType routineType, dynamic? parameters, DbSessionCallOptions options, CancellationToken cancellationToken = default);

    /// <summary>Gets the optional first routine result with per-call options.</summary>
    Task<T?> QueryFirstOrDefaultFromRoutineAsync<T>(string routineName, DbRoutineType routineType, dynamic? parameters, DbSessionCallOptions options, CancellationToken cancellationToken = default);

    /// <summary>Gets exactly one routine result with per-call options.</summary>
    Task<T> QuerySingleFromRoutineAsync<T>(string routineName, DbRoutineType routineType, dynamic? parameters, DbSessionCallOptions options, CancellationToken cancellationToken = default);

    /// <summary>Gets an optional single routine result with per-call options.</summary>
    Task<T?> QuerySingleOrDefaultFromRoutineAsync<T>(string routineName, DbRoutineType routineType, dynamic? parameters, DbSessionCallOptions options, CancellationToken cancellationToken = default);

    /// <summary>Gets multiple result sets with per-call options.</summary>
    Task<Dapper.SqlMapper.GridReader> QueryMultipleAsync(string statement, dynamic? parameters, DbSessionCallOptions options, CancellationToken cancellationToken = default);

    /// <summary>Gets multiple routine result sets with per-call options.</summary>
    Task<Dapper.SqlMapper.GridReader> QueryMultipleFromRoutineAsync(string routineName, DbRoutineType routineType, dynamic? parameters, DbSessionCallOptions options, CancellationToken cancellationToken = default);
}
