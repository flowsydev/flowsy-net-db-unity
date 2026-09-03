using System.Runtime.CompilerServices;

namespace Flowsy.Db.Unity;

public partial interface IDbSession
{
    /// <summary>Reads a query progressively without buffering all results in memory.</summary>
    IAsyncEnumerable<T> StreamAsync<T>(
        string statement,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default);

    /// <summary>Reads routine results progressively.</summary>
    IAsyncEnumerable<T> StreamFromRoutineAsync<T>(
        string routineName,
        DbRoutineType routineType,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default);

    /// <summary>Reads a query progressively with per-call options.</summary>
    IAsyncEnumerable<T> StreamAsync<T>(
        string statement,
        dynamic? parameters,
        DbSessionCallOptions options,
        CancellationToken cancellationToken = default);

    /// <summary>Reads routine results progressively with per-call options.</summary>
    IAsyncEnumerable<T> StreamFromRoutineAsync<T>(
        string routineName,
        DbRoutineType routineType,
        dynamic? parameters,
        DbSessionCallOptions options,
        CancellationToken cancellationToken = default);
}
