using Flowsy.Db.Unity.Extensions;

namespace Flowsy.Db.Unity;

public partial class DbAgent
{
    public IEnumerable<TReturn> GetFromStatement<TFirst, TSecond, TReturn>(
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TReturn> map,
        dynamic? parameters = null
    )
        => Connection.GetFromStatement(
            commandText,
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

    public IEnumerable<TReturn> GetFromStatement<TFirst, TSecond, TThird, TReturn>(
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TReturn> map,
        dynamic? parameters = null
    )
        => Connection.GetFromStatement(
            commandText,
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

    public IEnumerable<TReturn> GetFromStatement<TFirst, TSecond, TThird, TFourth, TReturn>(
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TReturn> map,
        dynamic? parameters = null
    )
        => Connection.GetFromStatement(
            commandText,
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

    public IEnumerable<TReturn> GetFromStatement<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map,
        dynamic? parameters = null
    )
        => Connection.GetFromStatement(
            commandText,
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

    public IEnumerable<TReturn> GetFromStatement<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map,
        dynamic? parameters = null
    )
        => Connection.GetFromStatement(
            commandText,
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

    public IEnumerable<TReturn> GetFromStatement<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map,
        dynamic? parameters = null
    )
        => Connection.GetFromStatement(
            commandText,
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

    public Task<IEnumerable<TReturn>> GetFromStatementAsync<TFirst, TSecond, TReturn>(
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TReturn> map,
        dynamic? parameters = null
    )
        => Connection.GetFromStatementAsync(
            commandText,
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

    public Task<IEnumerable<TReturn>> GetFromStatementAsync<TFirst, TSecond, TThird, TReturn>(
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TReturn> map,
        dynamic? parameters = null
    )
        => Connection.GetFromStatementAsync(
            commandText,
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

    public Task<IEnumerable<TReturn>> GetFromStatementAsync<TFirst, TSecond, TThird, TFourth, TReturn>(
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TReturn> map,
        dynamic? parameters = null
    )
        => Connection.GetFromStatementAsync(
            commandText,
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

    public Task<IEnumerable<TReturn>> GetFromStatementAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map,
        dynamic? parameters = null
    )
        => Connection.GetFromStatementAsync(
            commandText,
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

    public Task<IEnumerable<TReturn>> GetFromStatementAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map,
        dynamic? parameters = null
    )
        => Connection.GetFromStatementAsync(
            commandText,
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

    public Task<IEnumerable<TReturn>> GetFromStatementAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map,
        dynamic? parameters = null
    )
        => Connection.GetFromStatementAsync(
            commandText,
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