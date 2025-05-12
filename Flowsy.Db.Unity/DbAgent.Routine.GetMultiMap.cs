using Flowsy.Db.Unity.Extensions;

namespace Flowsy.Db.Unity;

public partial class DbAgent
{
    public IEnumerable<TReturn> GetFromRoutine<TFirst, TSecond, TReturn>(
        string routineName,
        string splitOn,
        Func<TFirst, TSecond, TReturn> map,
        dynamic? parameters = null
    )
        => GetFromRoutine(
            routineName,
            null,
            splitOn,
            map,
            parameters as object
        );

    public IEnumerable<TReturn> GetFromRoutine<TFirst, TSecond, TReturn>(
        string routineName,
        DbRoutineType? routineType,
        string splitOn,
        Func<TFirst, TSecond, TReturn> map,
        dynamic? parameters = null
    )
        => Connection.GetFromRoutine(
            routineName,
            routineType,
            splitOn,
            map,
            parameters as object,
            UnitOfWork?.Transaction,
            ConnectionOptions.Conventions,
            c => OnCommandExecuting(new DbCommandExecutingEventArgs(c, Connection, UnitOfWork?.Transaction)),
            (c, r) =>
            {
                OnCommandExecuted(new DbCommandExecutedEventArgs(c, Connection, UnitOfWork?.Transaction, r));
                return r;
            }
        );

    public IEnumerable<TReturn> GetFromRoutine<TFirst, TSecond, TThird, TReturn>(
        string routineName,
        string splitOn,
        Func<TFirst, TSecond, TThird, TReturn> map,
        dynamic? parameters = null
    )
        => GetFromRoutine(
            routineName,
            null,
            splitOn,
            map,
            parameters as object
        );

    public IEnumerable<TReturn> GetFromRoutine<TFirst, TSecond, TThird, TReturn>(
        string routineName,
        DbRoutineType? routineType,
        string splitOn,
        Func<TFirst, TSecond, TThird, TReturn> map,
        dynamic? parameters = null
    )
        => Connection.GetFromRoutine(
            routineName,
            routineType,
            splitOn,
            map,
            parameters as object,
            UnitOfWork?.Transaction,
            ConnectionOptions.Conventions,
            c => OnCommandExecuting(new DbCommandExecutingEventArgs(c, Connection, UnitOfWork?.Transaction)),
            (c, r) =>
            {
                OnCommandExecuted(new DbCommandExecutedEventArgs(c, Connection, UnitOfWork?.Transaction, r));
                return r;
            }
        );

    public IEnumerable<TReturn> GetFromRoutine<TFirst, TSecond, TThird, TFourth, TReturn>(
        string routineName,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TReturn> map,
        dynamic? parameters = null
    )
        => GetFromRoutine(
            routineName,
            null,
            splitOn,
            map,
            parameters as object
        );

    public IEnumerable<TReturn> GetFromRoutine<TFirst, TSecond, TThird, TFourth, TReturn>(
        string routineName,
        DbRoutineType? routineType,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TReturn> map,
        dynamic? parameters = null
    )
        => Connection.GetFromRoutine(
            routineName,
            routineType,
            splitOn,
            map,
            parameters as object,
            UnitOfWork?.Transaction,
            ConnectionOptions.Conventions,
            c => OnCommandExecuting(new DbCommandExecutingEventArgs(c, Connection, UnitOfWork?.Transaction)),
            (c, r) =>
            {
                OnCommandExecuted(new DbCommandExecutedEventArgs(c, Connection, UnitOfWork?.Transaction, r));
                return r;
            }
        );

    public IEnumerable<TReturn> GetFromRoutine<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(
        string routineName,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map,
        dynamic? parameters = null
    )
        => GetFromRoutine(
            routineName,
            null,
            splitOn,
            map,
            parameters as object
        );

    public IEnumerable<TReturn> GetFromRoutine<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(
        string routineName,
        DbRoutineType? routineType,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map,
        dynamic? parameters = null
    )
        => Connection.GetFromRoutine(
            routineName,
            routineType,
            splitOn,
            map,
            parameters as object,
            UnitOfWork?.Transaction,
            ConnectionOptions.Conventions,
            c => OnCommandExecuting(new DbCommandExecutingEventArgs(c, Connection, UnitOfWork?.Transaction)),
            (c, r) =>
            {
                OnCommandExecuted(new DbCommandExecutedEventArgs(c, Connection, UnitOfWork?.Transaction, r));
                return r;
            }
        );

    public IEnumerable<TReturn> GetFromRoutine<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(
        string routineName,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map,
        dynamic? parameters = null
    )
        => GetFromRoutine(
            routineName,
            null,
            splitOn,
            map,
            parameters as object
        );

    public IEnumerable<TReturn> GetFromRoutine<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(
        string routineName,
        DbRoutineType? routineType,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map,
        dynamic? parameters = null
    )
        => Connection.GetFromRoutine(
            routineName,
            routineType,
            splitOn,
            map,
            parameters as object,
            UnitOfWork?.Transaction,
            ConnectionOptions.Conventions,
            c => OnCommandExecuting(new DbCommandExecutingEventArgs(c, Connection, UnitOfWork?.Transaction)),
            (c, r) =>
            {
                OnCommandExecuted(new DbCommandExecutedEventArgs(c, Connection, UnitOfWork?.Transaction, r));
                return r;
            }
        );

    public IEnumerable<TReturn> GetFromRoutine<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(
        string routineName,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map,
        dynamic? parameters = null
    )
        => GetFromRoutine(
            routineName,
            null,
            splitOn,
            map,
            parameters as object
        );

    public IEnumerable<TReturn> GetFromRoutine<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(
        string routineName,
        DbRoutineType? routineType,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map,
        dynamic? parameters = null
    )
        => Connection.GetFromRoutine(
            routineName,
            routineType,
            splitOn,
            map,
            parameters as object,
            UnitOfWork?.Transaction,
            ConnectionOptions.Conventions,
            c => OnCommandExecuting(new DbCommandExecutingEventArgs(c, Connection, UnitOfWork?.Transaction)),
            (c, r) =>
            {
                OnCommandExecuted(new DbCommandExecutedEventArgs(c, Connection, UnitOfWork?.Transaction, r));
                return r;
            }
        );
    
    public Task<IEnumerable<TReturn>> GetFromRoutineAsync<TFirst, TSecond, TReturn>(
        string routineName,
        string splitOn,
        Func<TFirst, TSecond, TReturn> map,
        dynamic? parameters = null
    )
        => GetFromRoutineAsync(
            routineName,
            null,
            splitOn,
            map,
            parameters as object
        );

    public Task<IEnumerable<TReturn>> GetFromRoutineAsync<TFirst, TSecond, TReturn>(
        string routineName,
        DbRoutineType? routineType,
        string splitOn,
        Func<TFirst, TSecond, TReturn> map,
        dynamic? parameters = null
    )
        => Connection.GetFromRoutineAsync(
            routineName,
            routineType,
            splitOn,
            map,
            parameters as object,
            UnitOfWork?.Transaction,
            ConnectionOptions.Conventions,
            c => OnCommandExecuting(new DbCommandExecutingEventArgs(c, Connection, UnitOfWork?.Transaction)),
            (c, r) =>
            {
                OnCommandExecuted(new DbCommandExecutedEventArgs(c, Connection, UnitOfWork?.Transaction, r));
                return Task.FromResult(r);
            }
        );

    public Task<IEnumerable<TReturn>> GetFromRoutineAsync<TFirst, TSecond, TThird, TReturn>(
        string routineName,
        string splitOn,
        Func<TFirst, TSecond, TThird, TReturn> map,
        dynamic? parameters = null
    )
        => GetFromRoutineAsync(
            routineName,
            null,
            splitOn,
            map,
            parameters as object
        );

    public Task<IEnumerable<TReturn>> GetFromRoutineAsync<TFirst, TSecond, TThird, TReturn>(
        string routineName,
        DbRoutineType? routineType,
        string splitOn,
        Func<TFirst, TSecond, TThird, TReturn> map,
        dynamic? parameters = null
    )
        => Connection.GetFromRoutineAsync(
            routineName,
            routineType,
            splitOn,
            map,
            parameters as object,
            UnitOfWork?.Transaction,
            ConnectionOptions.Conventions,
            c => OnCommandExecuting(new DbCommandExecutingEventArgs(c, Connection, UnitOfWork?.Transaction)),
            (c, r) =>
            {
                OnCommandExecuted(new DbCommandExecutedEventArgs(c, Connection, UnitOfWork?.Transaction, r));
                return Task.FromResult(r);
            }
        );

    public Task<IEnumerable<TReturn>> GetFromRoutineAsync<TFirst, TSecond, TThird, TFourth, TReturn>(
        string routineName,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TReturn> map,
        dynamic? parameters = null
    )
        => GetFromRoutineAsync(
            routineName,
            null,
            splitOn,
            map,
            parameters as object
        );

    public Task<IEnumerable<TReturn>> GetFromRoutineAsync<TFirst, TSecond, TThird, TFourth, TReturn>(
        string routineName,
        DbRoutineType? routineType,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TReturn> map,
        dynamic? parameters = null
    )
        => Connection.GetFromRoutineAsync(
            routineName,
            routineType,
            splitOn,
            map,
            parameters as object,
            UnitOfWork?.Transaction,
            ConnectionOptions.Conventions,
            c => OnCommandExecuting(new DbCommandExecutingEventArgs(c, Connection, UnitOfWork?.Transaction)),
            (c, r) =>
            {
                OnCommandExecuted(new DbCommandExecutedEventArgs(c, Connection, UnitOfWork?.Transaction, r));
                return Task.FromResult(r);
            }
        );

    public Task<IEnumerable<TReturn>> GetFromRoutineAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(
        string routineName,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map,
        dynamic? parameters = null
    )
        => GetFromRoutineAsync(
            routineName,
            null,
            splitOn,
            map,
            parameters as object
        );

    public Task<IEnumerable<TReturn>> GetFromRoutineAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(
        string routineName,
        DbRoutineType? routineType,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map,
        dynamic? parameters = null
    )
        => Connection.GetFromRoutineAsync(
            routineName,
            routineType,
            splitOn,
            map,
            parameters as object,
            UnitOfWork?.Transaction,
            ConnectionOptions.Conventions,
            c => OnCommandExecuting(new DbCommandExecutingEventArgs(c, Connection, UnitOfWork?.Transaction)),
            (c, r) =>
            {
                OnCommandExecuted(new DbCommandExecutedEventArgs(c, Connection, UnitOfWork?.Transaction, r));
                return Task.FromResult(r);
            }
        );

    public Task<IEnumerable<TReturn>> GetFromRoutineAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(
        string routineName,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map,
        dynamic? parameters = null
    )
        => GetFromRoutineAsync(
            routineName,
            null,
            splitOn,
            map,
            parameters as object
        );

    public Task<IEnumerable<TReturn>> GetFromRoutineAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(
        string routineName,
        DbRoutineType? routineType,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map,
        dynamic? parameters = null
    )
        => Connection.GetFromRoutineAsync(
            routineName,
            routineType,
            splitOn,
            map,
            parameters as object,
            UnitOfWork?.Transaction,
            ConnectionOptions.Conventions,
            c => OnCommandExecuting(new DbCommandExecutingEventArgs(c, Connection, UnitOfWork?.Transaction)),
            (c, r) =>
            {
                OnCommandExecuted(new DbCommandExecutedEventArgs(c, Connection, UnitOfWork?.Transaction, r));
                return Task.FromResult(r);
            }
        );

    public Task<IEnumerable<TReturn>> GetFromRoutineAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(
        string routineName,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map,
        dynamic? parameters = null
    )
        => GetFromRoutineAsync(
            routineName,
            null,
            splitOn,
            map,
            parameters as object
        );

    public Task<IEnumerable<TReturn>> GetFromRoutineAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(
        string routineName,
        DbRoutineType? routineType,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map,
        dynamic? parameters = null
    )
        => Connection.GetFromRoutineAsync(
            routineName,
            routineType,
            splitOn,
            map,
            parameters as object,
            UnitOfWork?.Transaction,
            ConnectionOptions.Conventions,
            c => OnCommandExecuting(new DbCommandExecutingEventArgs(c, Connection, UnitOfWork?.Transaction)),
            (c, r) =>
            {
                OnCommandExecuted(new DbCommandExecutedEventArgs(c, Connection, UnitOfWork?.Transaction, r));
                return Task.FromResult(r);
            }
        );
}