namespace Flowsy.Db.Unity;

public partial class DbSession
{
    /// <inheritdoc />
    public Task<int> ExecuteAsync(string statement, dynamic? parameters, DbSessionCallOptions options, CancellationToken cancellationToken = default)
        => WithCallOptionsAsync<int>(options, () => ExecuteAsync(statement, parameters as object, cancellationToken));

    /// <inheritdoc />
    public Task<int> ExecuteRoutineAsync(string routineName, DbRoutineType routineType, dynamic? parameters, DbSessionCallOptions options, CancellationToken cancellationToken = default)
        => WithCallOptionsAsync<int>(options, () => ExecuteRoutineAsync(routineName, routineType, parameters as object, cancellationToken));

    /// <inheritdoc />
    public Task<IEnumerable<T>> QueryAsync<T>(string statement, dynamic? parameters, DbSessionCallOptions options, CancellationToken cancellationToken = default)
        => WithCallOptionsAsync<IEnumerable<T>>(options, () => QueryAsync<T>(statement, parameters as object, cancellationToken));

    /// <inheritdoc />
    public Task<IEnumerable<T>> QueryFromRoutineAsync<T>(string routineName, DbRoutineType routineType, dynamic? parameters, DbSessionCallOptions options, CancellationToken cancellationToken = default)
        => WithCallOptionsAsync<IEnumerable<T>>(options, () => QueryFromRoutineAsync<T>(routineName, routineType, parameters as object, cancellationToken));

    /// <inheritdoc />
    public Task<T> QueryFirstAsync<T>(string statement, dynamic? parameters, DbSessionCallOptions options, CancellationToken cancellationToken = default)
        => WithCallOptionsAsync<T>(options, () => QueryFirstAsync<T>(statement, parameters as object, cancellationToken));

    /// <inheritdoc />
    public Task<T?> QueryFirstOrDefaultAsync<T>(string statement, dynamic? parameters, DbSessionCallOptions options, CancellationToken cancellationToken = default)
        => WithCallOptionsAsync<T?>(options, () => QueryFirstOrDefaultAsync<T>(statement, parameters as object, cancellationToken));

    /// <inheritdoc />
    public Task<T> QuerySingleAsync<T>(string statement, dynamic? parameters, DbSessionCallOptions options, CancellationToken cancellationToken = default)
        => WithCallOptionsAsync<T>(options, () => QuerySingleAsync<T>(statement, parameters as object, cancellationToken));

    /// <inheritdoc />
    public Task<T?> QuerySingleOrDefaultAsync<T>(string statement, dynamic? parameters, DbSessionCallOptions options, CancellationToken cancellationToken = default)
        => WithCallOptionsAsync<T?>(options, () => QuerySingleOrDefaultAsync<T>(statement, parameters as object, cancellationToken));

    /// <inheritdoc />
    public Task<T> QueryFirstFromRoutineAsync<T>(string routineName, DbRoutineType routineType, dynamic? parameters, DbSessionCallOptions options, CancellationToken cancellationToken = default)
        => WithCallOptionsAsync<T>(options, () => QueryFirstFromRoutineAsync<T>(routineName, routineType, parameters as object, cancellationToken));

    /// <inheritdoc />
    public Task<T?> QueryFirstOrDefaultFromRoutineAsync<T>(string routineName, DbRoutineType routineType, dynamic? parameters, DbSessionCallOptions options, CancellationToken cancellationToken = default)
        => WithCallOptionsAsync<T?>(options, () => QueryFirstOrDefaultFromRoutineAsync<T>(routineName, routineType, parameters as object, cancellationToken));

    /// <inheritdoc />
    public Task<T> QuerySingleFromRoutineAsync<T>(string routineName, DbRoutineType routineType, dynamic? parameters, DbSessionCallOptions options, CancellationToken cancellationToken = default)
        => WithCallOptionsAsync<T>(options, () => QuerySingleFromRoutineAsync<T>(routineName, routineType, parameters as object, cancellationToken));

    /// <inheritdoc />
    public Task<T?> QuerySingleOrDefaultFromRoutineAsync<T>(string routineName, DbRoutineType routineType, dynamic? parameters, DbSessionCallOptions options, CancellationToken cancellationToken = default)
        => WithCallOptionsAsync<T?>(options, () => QuerySingleOrDefaultFromRoutineAsync<T>(routineName, routineType, parameters as object, cancellationToken));

    /// <inheritdoc />
    public Task<Dapper.SqlMapper.GridReader> QueryMultipleAsync(string statement, dynamic? parameters, DbSessionCallOptions options, CancellationToken cancellationToken = default)
        => WithCallOptionsAsync<Dapper.SqlMapper.GridReader>(options, () => QueryMultipleAsync(statement, parameters as object, cancellationToken));

    /// <inheritdoc />
    public Task<Dapper.SqlMapper.GridReader> QueryMultipleFromRoutineAsync(string routineName, DbRoutineType routineType, dynamic? parameters, DbSessionCallOptions options, CancellationToken cancellationToken = default)
        => WithCallOptionsAsync<Dapper.SqlMapper.GridReader>(options, () => QueryMultipleFromRoutineAsync(routineName, routineType, parameters as object, cancellationToken));
}
