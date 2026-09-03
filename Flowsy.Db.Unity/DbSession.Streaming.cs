using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Dapper;
using Microsoft.Extensions.Logging;

namespace Flowsy.Db.Unity;

public partial class DbSession
{
    /// <inheritdoc />
    public IAsyncEnumerable<T> StreamAsync<T>(
        string statement,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default)
        => StreamCoreAsync<T>(statement, parameters as object, null, cancellationToken);

    /// <inheritdoc />
    public IAsyncEnumerable<T> StreamAsync<T>(
        string statement,
        dynamic? parameters,
        DbSessionCallOptions options,
        CancellationToken cancellationToken = default)
        => StreamCoreAsync<T>(statement, parameters as object, options, cancellationToken);

    private async IAsyncEnumerable<T> StreamCoreAsync<T>(
        string statement,
        object? parameters,
        DbSessionCallOptions? options,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        EnsureNotDisposed();
        await EnsureOpenConnectionAsync(cancellationToken);
        if (_connection is not DbConnection dbConnection)
            throw new NotSupportedException("Asynchronous streaming requires a connection derived from DbConnection.");

        var previousOptions = _callOptions.Value;
        _callOptions.Value = options;
        CommandDefinition command;
        try
        {
            command = BuildCommandDefinition(
                statement, parameters, CommandType.Text,
                Configuration.Conventions.Commands, cancellationToken);
        }
        finally
        {
            _callOptions.Value = previousOptions;
        }

        using var activity = DbDiagnostics.ActivitySource.StartActivity("db.stream", ActivityKind.Client);
        activity?.SetTag("db.system.name", Configuration.Provider.Family.ToString());
        activity?.SetTag("db.namespace", ConnectionKey);
        activity?.SetTag("db.operation.name", "stream");
        activity?.SetTag("db.session.id", SessionId);
        activity?.SetTag("db.operation.tag", options?.SanitizedTag);
        var stopwatch = Stopwatch.StartNew();
        DbDiagnostics.Commands.Add(1, CreateMetricTags("stream"));
        var operationId = CreateOperationId();
        _logger?.Log(Configuration.LogLevel,
            "[ SESSION:{SessionId} > OP:{OperationId} ] Starting database result stream",
            SessionId, operationId);
        DbDataReader reader;
        try
        {
            reader = await dbConnection.ExecuteReaderAsync(command);
        }
        catch (Exception exception)
        {
            activity?.SetStatus(ActivityStatusCode.Error, exception.GetType().Name);
            DbDiagnostics.Errors.Add(1, CreateMetricTags("stream"));
            _logger?.LogError(exception,
                "[ SESSION:{SessionId} > OP:{OperationId} ] Error starting database result stream",
                SessionId, operationId);
            throw;
        }
        await using var ownedReader = reader;
        var parser = reader.GetRowParser<T>();
        var failed = false;
        try
        {
            while (true)
            {
                T item;
                try
                {
                    if (!await reader.ReadAsync(cancellationToken))
                        break;
                    item = parser(reader);
                }
                catch (Exception exception)
                {
                    failed = true;
                    activity?.SetStatus(ActivityStatusCode.Error, exception.GetType().Name);
                    DbDiagnostics.Errors.Add(1, CreateMetricTags("stream"));
                    _logger?.LogError(exception,
                        "[ SESSION:{SessionId} > OP:{OperationId} ] Error consuming database result stream",
                        SessionId, operationId);
                    throw;
                }
                yield return item;
            }
        }
        finally
        {
            stopwatch.Stop();
            DbDiagnostics.Duration.Record(stopwatch.Elapsed.TotalMilliseconds, CreateMetricTags("stream"));
            if (!failed)
                _logger?.Log(Configuration.LogLevel,
                    "[ SESSION:{SessionId} > OP:{OperationId} ] Database result stream completed",
                    SessionId, operationId);
            if (Configuration.SlowOperationThreshold is { } threshold && stopwatch.Elapsed >= threshold)
                _logger?.LogWarning(
                    "[ SESSION:{SessionId} > OP:{OperationId} ] Slow database stream took {ElapsedMilliseconds} ms",
                    SessionId, CreateOperationId(), stopwatch.Elapsed.TotalMilliseconds);
        }
    }

    /// <inheritdoc />
    public IAsyncEnumerable<T> StreamFromRoutineAsync<T>(
        string routineName,
        DbRoutineType routineType,
        dynamic? parameters = null,
        CancellationToken cancellationToken = default)
        => StreamRoutineCoreAsync<T>(routineName, routineType, parameters as object, null, cancellationToken);

    /// <inheritdoc />
    public IAsyncEnumerable<T> StreamFromRoutineAsync<T>(
        string routineName,
        DbRoutineType routineType,
        dynamic? parameters,
        DbSessionCallOptions options,
        CancellationToken cancellationToken = default)
        => StreamRoutineCoreAsync<T>(routineName, routineType, parameters as object, options, cancellationToken);

    private async IAsyncEnumerable<T> StreamRoutineCoreAsync<T>(
        string routineName,
        DbRoutineType routineType,
        object? parameters,
        DbSessionCallOptions? options,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        EnsureNotDisposed();
        await EnsureOpenConnectionAsync(cancellationToken);
        if (_connection is not DbConnection dbConnection)
            throw new NotSupportedException("Asynchronous streaming requires a connection derived from DbConnection.");

        var (routineConvention, commandConvention) = Configuration.Conventions;
        var previousOptions = _callOptions.Value;
        _callOptions.Value = options;
        CommandDefinition command;
        try
        {
            command = BuildCommandDefinition(
                routineName, routineType, parameters, true,
                routineConvention, commandConvention, cancellationToken);
        }
        finally
        {
            _callOptions.Value = previousOptions;
        }

        using var activity = DbDiagnostics.ActivitySource.StartActivity("db.stream_routine", ActivityKind.Client);
        activity?.SetTag("db.system.name", Configuration.Provider.Family.ToString());
        activity?.SetTag("db.namespace", ConnectionKey);
        activity?.SetTag("db.operation.name", "stream_routine");
        activity?.SetTag("db.session.id", SessionId);
        activity?.SetTag("db.routine.name", routineName);
        activity?.SetTag("db.operation.tag", options?.SanitizedTag);
        var stopwatch = Stopwatch.StartNew();
        DbDiagnostics.Commands.Add(1, CreateMetricTags("stream_routine"));
        var operationId = CreateOperationId();
        _logger?.Log(Configuration.LogLevel,
            "[ SESSION:{SessionId} > OP:{OperationId} ] Starting database routine result stream",
            SessionId, operationId);
        DbDataReader reader;
        try
        {
            reader = await dbConnection.ExecuteReaderAsync(command);
        }
        catch (Exception exception)
        {
            activity?.SetStatus(ActivityStatusCode.Error, exception.GetType().Name);
            DbDiagnostics.Errors.Add(1, CreateMetricTags("stream_routine"));
            _logger?.LogError(exception,
                "[ SESSION:{SessionId} > OP:{OperationId} ] Error starting database routine result stream",
                SessionId, operationId);
            throw;
        }
        await using var ownedReader = reader;
        var parser = reader.GetRowParser<T>();
        var failed = false;
        try
        {
            while (true)
            {
                T item;
                try
                {
                    if (!await reader.ReadAsync(cancellationToken))
                        break;
                    item = parser(reader);
                }
                catch (Exception exception)
                {
                    failed = true;
                    activity?.SetStatus(ActivityStatusCode.Error, exception.GetType().Name);
                    DbDiagnostics.Errors.Add(1, CreateMetricTags("stream_routine"));
                    _logger?.LogError(exception,
                        "[ SESSION:{SessionId} > OP:{OperationId} ] Error consuming database routine result stream",
                        SessionId, operationId);
                    throw;
                }
                yield return item;
            }
        }
        finally
        {
            stopwatch.Stop();
            DbDiagnostics.Duration.Record(stopwatch.Elapsed.TotalMilliseconds, CreateMetricTags("stream_routine"));
            if (!failed)
                _logger?.Log(Configuration.LogLevel,
                    "[ SESSION:{SessionId} > OP:{OperationId} ] Database routine result stream completed",
                    SessionId, operationId);
            if (Configuration.SlowOperationThreshold is { } threshold && stopwatch.Elapsed >= threshold)
                _logger?.LogWarning(
                    "[ SESSION:{SessionId} > OP:{OperationId} ] Slow database routine stream took {ElapsedMilliseconds} ms",
                    SessionId, CreateOperationId(), stopwatch.Elapsed.TotalMilliseconds);
        }
    }
}
