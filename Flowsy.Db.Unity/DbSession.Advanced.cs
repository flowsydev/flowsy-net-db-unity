using System.Data;
using Microsoft.Extensions.Logging;

namespace Flowsy.Db.Unity;

public partial class DbSession
{
    /// <inheritdoc />
    public Task InTransactionAsync(
        Func<IDbSession, CancellationToken, Task> work,
        CancellationToken cancellationToken = default)
        => InTransactionAsync<object?>(async (session, token) =>
        {
            await work(session, token);
            return null;
        }, cancellationToken);

    /// <inheritdoc />
    public async Task<TResult> InTransactionAsync<TResult>(
        Func<IDbSession, CancellationToken, Task<TResult>> work,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(work);
        EnsureNotDisposed();
        EnsureNotInTransaction();
        await BeginTransactionAsync(cancellationToken: cancellationToken);
        try
        {
            var result = await work(this, cancellationToken);
            await CommitTransactionAsync(cancellationToken);
            return result;
        }
        catch
        {
            await TryRollbackTransactionAsync(CancellationToken.None);
            throw;
        }
    }

    /// <inheritdoc />
    public Task InExistingOrNewTransactionAsync(
        Func<IDbSession, CancellationToken, Task> work,
        CancellationToken cancellationToken = default)
        => InExistingOrNewTransactionAsync<object?>(async (session, token) =>
        {
            await work(session, token);
            return null;
        }, cancellationToken);

    /// <inheritdoc />
    public async Task<TResult> InExistingOrNewTransactionAsync<TResult>(
        Func<IDbSession, CancellationToken, Task<TResult>> work,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(work);
        EnsureNotDisposed();
        if (InTransaction)
            return await work(this, cancellationToken);
        return await InTransactionAsync(work, cancellationToken);
    }

    /// <inheritdoc />
    public Task WithConnectionAsync(
        Func<IDbConnection, IDbTransaction?, CancellationToken, Task> work,
        CancellationToken cancellationToken = default)
        => WithConnectionAsync<object?>(async (connection, transaction, token) =>
        {
            await work(connection, transaction, token);
            return null;
        }, cancellationToken);

    /// <inheritdoc />
    public Task<TResult> WithConnectionAsync<TResult>(
        Func<IDbConnection, IDbTransaction?, CancellationToken, Task<TResult>> work,
        CancellationToken cancellationToken = default)
        => WithConnectionCoreAsync(work, cancellationToken);

    /// <inheritdoc />
    public Task WithConnectionAsync<TConnection>(
        Func<TConnection, IDbTransaction?, CancellationToken, Task> work,
        CancellationToken cancellationToken = default)
        where TConnection : class, IDbConnection
        => WithConnectionAsync<TConnection, object?>(async (connection, transaction, token) =>
        {
            await work(connection, transaction, token);
            return null;
        }, cancellationToken);

    /// <inheritdoc />
    public Task<TResult> WithConnectionAsync<TConnection, TResult>(
        Func<TConnection, IDbTransaction?, CancellationToken, Task<TResult>> work,
        CancellationToken cancellationToken = default)
        where TConnection : class, IDbConnection
    {
        ArgumentNullException.ThrowIfNull(work);
        EnsureNotDisposed();
        if (_connection is not TConnection typedConnection)
            throw new InvalidOperationException(
                $"The connection '{ConnectionKey}' has type '{_connection.GetType().FullName}', instead of the expected type '{typeof(TConnection).FullName}'.");
        return WithConnectionCoreAsync((_, transaction, token) => work(typedConnection, transaction, token), cancellationToken);
    }

    private async Task<TResult> WithConnectionCoreAsync<TResult>(
        Func<IDbConnection, IDbTransaction?, CancellationToken, Task<TResult>> work,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(work);
        EnsureNotDisposed();
        await EnsureOpenConnectionAsync(cancellationToken);
        var operationId = CreateOperationId();
        _logger?.Log(Configuration.LogLevel,
            "[ SESSION:{SessionId} > OP:{OperationId} ] Executing callback with session connection",
            SessionId, operationId);
        try
        {
            var result = await work(_connection, _transaction, cancellationToken);
            _logger?.Log(Configuration.LogLevel,
                "[ SESSION:{SessionId} > OP:{OperationId} ] Callback with session connection completed",
                SessionId, operationId);
            return result;
        }
        catch (Exception exception)
        {
            _logger?.LogError(exception,
                "[ SESSION:{SessionId} > OP:{OperationId} ] Error executing callback with session connection",
                SessionId, operationId);
            throw;
        }
    }

    /// <inheritdoc />
    public Task WithSettingsAsync(
        IEnumerable<DbSessionSetting> settings,
        Func<IDbSession, CancellationToken, Task> work,
        CancellationToken cancellationToken = default)
        => WithSettingsAsync<object?>(settings, async (session, token) =>
        {
            await work(session, token);
            return null;
        }, cancellationToken);

    /// <inheritdoc />
    public async Task<TResult> WithSettingsAsync<TResult>(
        IEnumerable<DbSessionSetting> settings,
        Func<IDbSession, CancellationToken, Task<TResult>> work,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(work);
        EnsureNotDisposed();
        var commands = settings.Select(x => _sessionSettingFormatter.Format(x, Configuration)).ToArray();
        var appliedCommands = new Stack<DbSessionSettingCommand>();
        await EnsureOpenConnectionAsync(cancellationToken);
        try
        {
            foreach (var command in commands)
            {
                await ExecuteCommandAsync(command.ApplyStatement, cancellationToken: cancellationToken);
                appliedCommands.Push(command);
            }

            return await work(this, cancellationToken);
        }
        finally
        {
            while (appliedCommands.TryPop(out var command))
                await ExecuteCommandAsync(command.CleanupStatement, cancellationToken: CancellationToken.None);
        }
    }

    private void EnsureNotDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }
}
