namespace Flowsy.Db.Unity;

public partial interface IDbAgent
{
    IEnumerable<TReturn> GetFromStatement<TFirst, TSecond, TReturn>(
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TReturn> map,
        dynamic? parameters = null
        );
    
    IEnumerable<TReturn> GetFromStatement<TFirst, TSecond, TThird, TReturn>(
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TReturn> map,
        dynamic? parameters = null
        );
    
    IEnumerable<TReturn> GetFromStatement<TFirst, TSecond, TThird, TFourth, TReturn>(
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TReturn> map,
        dynamic? parameters = null
        );
    
    IEnumerable<TReturn> GetFromStatement<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map,
        dynamic? parameters = null
        );
    
    IEnumerable<TReturn> GetFromStatement<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map,
        dynamic? parameters = null
        );
    
    IEnumerable<TReturn> GetFromStatement<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map,
        dynamic? parameters = null
        );
    
    Task<IEnumerable<TReturn>> GetFromStatementAsync<TFirst, TSecond, TReturn>(
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TReturn> map,
        dynamic? parameters = null
        );
    
    Task<IEnumerable<TReturn>> GetFromStatementAsync<TFirst, TSecond, TThird, TReturn>(
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TReturn> map,
        dynamic? parameters = null
        );
    
    Task<IEnumerable<TReturn>> GetFromStatementAsync<TFirst, TSecond, TThird, TFourth, TReturn>(
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TReturn> map,
        dynamic? parameters = null
        );
    
    Task<IEnumerable<TReturn>> GetFromStatementAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map,
        dynamic? parameters = null
        );
    
    Task<IEnumerable<TReturn>> GetFromStatementAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map,
        dynamic? parameters = null
        );
    
    Task<IEnumerable<TReturn>> GetFromStatementAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map,
        dynamic? parameters = null
        );
}