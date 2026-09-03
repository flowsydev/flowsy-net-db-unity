using System.Data;

namespace Flowsy.Db.Unity;

public partial interface IDbSession
{
    /// <summary>Executes work within a new transaction.</summary>
    Task InTransactionAsync(Func<IDbSession, CancellationToken, Task> work, CancellationToken cancellationToken = default);

    /// <summary>Executes work that returns a result within a new transaction.</summary>
    Task<TResult> InTransactionAsync<TResult>(Func<IDbSession, CancellationToken, Task<TResult>> work, CancellationToken cancellationToken = default);

    /// <summary>Reuses the active transaction or creates one when none exists.</summary>
    Task InExistingOrNewTransactionAsync(Func<IDbSession, CancellationToken, Task> work, CancellationToken cancellationToken = default);

    /// <summary>Reuses the active transaction or creates one when none exists, and returns a result.</summary>
    Task<TResult> InExistingOrNewTransactionAsync<TResult>(Func<IDbSession, CancellationToken, Task<TResult>> work, CancellationToken cancellationToken = default);

    /// <summary>Provides the underlying connection and transaction to a callback.</summary>
    Task WithConnectionAsync(Func<IDbConnection, IDbTransaction?, CancellationToken, Task> work, CancellationToken cancellationToken = default);

    /// <summary>Provides the underlying connection and transaction to a callback that returns a result.</summary>
    Task<TResult> WithConnectionAsync<TResult>(Func<IDbConnection, IDbTransaction?, CancellationToken, Task<TResult>> work, CancellationToken cancellationToken = default);

    /// <summary>Provides the underlying connection after validating its concrete type.</summary>
    Task WithConnectionAsync<TConnection>(Func<TConnection, IDbTransaction?, CancellationToken, Task> work, CancellationToken cancellationToken = default)
        where TConnection : class, IDbConnection;

    /// <summary>Provides the underlying connection after validating its concrete type, and returns a result.</summary>
    Task<TResult> WithConnectionAsync<TConnection, TResult>(Func<TConnection, IDbTransaction?, CancellationToken, Task<TResult>> work, CancellationToken cancellationToken = default)
        where TConnection : class, IDbConnection;

    /// <summary>Applies allowed session settings during a callback and guarantees cleanup.</summary>
    Task WithSettingsAsync(IEnumerable<DbSessionSetting> settings, Func<IDbSession, CancellationToken, Task> work, CancellationToken cancellationToken = default);

    /// <summary>Applies allowed session settings during a callback that returns a result and guarantees cleanup.</summary>
    Task<TResult> WithSettingsAsync<TResult>(IEnumerable<DbSessionSetting> settings, Func<IDbSession, CancellationToken, Task<TResult>> work, CancellationToken cancellationToken = default);
}
