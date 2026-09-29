using System.Data;
using System.Data.Common;
using System.Runtime.ExceptionServices;
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
        foreach (var command in commands)
        {
            if ((command.ReadStatement is null) != (command.RestoreStatementFactory is null))
                throw new InvalidOperationException("A setting command must provide both a read statement and a restoration factory.");
        }
        var cleanupStatements = new Stack<string>();
        Exception? operationException = null;
        await EnsureOpenConnectionAsync(cancellationToken);
        try
        {
            foreach (var command in commands)
            {
                var cleanup = command.ReadStatement is not null
                    ? command.RestoreStatementFactory!(await ReadSettingValueAsync(command.ReadStatement, cancellationToken))
                    : command.CleanupStatement;
                await ExecuteCommandAsync(command.ApplyStatement, cancellationToken: cancellationToken);
                cleanupStatements.Push(cleanup);
            }

            return await work(this, cancellationToken);
        }
        catch (Exception exception)
        {
            operationException = exception;
            throw;
        }
        finally
        {
            var cleanupExceptions = new List<Exception>();
            while (cleanupStatements.TryPop(out var statement))
            {
                try
                {
                    await ExecuteCommandAsync(statement, cancellationToken: CancellationToken.None);
                }
                catch (Exception exception)
                {
                    cleanupExceptions.Add(exception);
                }
            }

            if (cleanupExceptions.Count > 0)
            {
                if (operationException is not null)
                    cleanupExceptions.Insert(0, operationException);
                if (cleanupExceptions.Count == 1)
                    ExceptionDispatchInfo.Capture(cleanupExceptions[0]).Throw();
                throw new AggregateException("One or more session settings could not be restored.", cleanupExceptions);
            }
        }
    }

    private async Task<object?> ReadSettingValueAsync(string statement, CancellationToken cancellationToken)
    {
        using var command = _connection.CreateCommand();
        command.CommandText = statement;
        command.CommandType = CommandType.Text;
        command.Transaction = _transaction;
        if (Configuration.Conventions.Commands.Timeout is { } timeout)
            command.CommandTimeout = timeout;
        var operationId = CreateOperationId();
        _logger?.Log(Configuration.LogLevel,
            "[ SESSION:{SessionId} > OP:{OperationId} ] Reading current session setting with {Statement}",
            SessionId, operationId, statement);
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var value = command is DbCommand dbCommand
                ? await dbCommand.ExecuteScalarAsync(cancellationToken)
                : command.ExecuteScalar();
            if (value is null)
                throw new InvalidOperationException("The session-setting query did not return a value.");
            _logger?.Log(Configuration.LogLevel,
                "[ SESSION:{SessionId} > OP:{OperationId} ] Current session setting read successfully",
                SessionId, operationId);
            return value is DBNull ? null : value;
        }
        catch (Exception exception)
        {
            _logger?.LogError(exception,
                "[ SESSION:{SessionId} > OP:{OperationId} ] Error reading current session setting",
                SessionId, operationId);
            throw;
        }
    }

    private void EnsureNotDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }
}
