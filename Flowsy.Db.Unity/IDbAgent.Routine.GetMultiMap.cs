namespace Flowsy.Db.Unity;

public partial interface IDbAgent
{
    IEnumerable<TReturn> GetFromRoutine<TFirst, TSecond, TReturn>(
        string routineName,
        string splitOn,
        Func<TFirst, TSecond, TReturn> map,
        dynamic? parameters = null
        );
    
    IEnumerable<TReturn> GetFromRoutine<TFirst, TSecond, TReturn>(
        string routineName,
        DbRoutineType? routineType,
        string splitOn,
        Func<TFirst, TSecond, TReturn> map,
        dynamic? parameters = null
        );
    
    IEnumerable<TReturn> GetFromRoutine<TFirst, TSecond, TThird, TReturn>(
        string routineName,
        string splitOn,
        Func<TFirst, TSecond, TThird, TReturn> map,
        dynamic? parameters = null
        );
    
    IEnumerable<TReturn> GetFromRoutine<TFirst, TSecond, TThird, TReturn>(
        string routineName,
        DbRoutineType? routineType,
        string splitOn,
        Func<TFirst, TSecond, TThird, TReturn> map,
        dynamic? parameters = null
        );
    
    IEnumerable<TReturn> GetFromRoutine<TFirst, TSecond, TThird, TFourth, TReturn>(
        string routineName,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TReturn> map,
        dynamic? parameters = null
        );
    
    IEnumerable<TReturn> GetFromRoutine<TFirst, TSecond, TThird, TFourth, TReturn>(
        string routineName,
        DbRoutineType? routineType,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TReturn> map,
        dynamic? parameters = null
        );
    
    IEnumerable<TReturn> GetFromRoutine<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(
        string routineName,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map,
        dynamic? parameters = null
        );
    
    IEnumerable<TReturn> GetFromRoutine<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(
        string routineName,
        DbRoutineType? routineType,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map,
        dynamic? parameters = null
        );
    
    IEnumerable<TReturn> GetFromRoutine<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(
        string routineName,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map,
        dynamic? parameters = null
        );
    
    IEnumerable<TReturn> GetFromRoutine<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(
        string routineName,
        DbRoutineType? routineType,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map,
        dynamic? parameters = null
        );
    
    IEnumerable<TReturn> GetFromRoutine<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(
        string routineName,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map,
        dynamic? parameters = null
        );
    
    IEnumerable<TReturn> GetFromRoutine<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(
        string routineName,
        DbRoutineType? routineType,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map,
        dynamic? parameters = null
        );
    
    Task<IEnumerable<TReturn>> GetFromRoutineAsync<TFirst, TSecond, TReturn>(
        string routineName,
        string splitOn,
        Func<TFirst, TSecond, TReturn> map,
        dynamic? parameters = null
        );
    
    Task<IEnumerable<TReturn>> GetFromRoutineAsync<TFirst, TSecond, TReturn>(
        string routineName,
        DbRoutineType? routineType,
        string splitOn,
        Func<TFirst, TSecond, TReturn> map,
        dynamic? parameters = null
        );
    
    Task<IEnumerable<TReturn>> GetFromRoutineAsync<TFirst, TSecond, TThird, TReturn>(
        string routineName,
        string splitOn,
        Func<TFirst, TSecond, TThird, TReturn> map,
        dynamic? parameters = null
        );
    
    Task<IEnumerable<TReturn>> GetFromRoutineAsync<TFirst, TSecond, TThird, TReturn>(
        string routineName,
        DbRoutineType? routineType,
        string splitOn,
        Func<TFirst, TSecond, TThird, TReturn> map,
        dynamic? parameters = null
        );
    
    Task<IEnumerable<TReturn>> GetFromRoutineAsync<TFirst, TSecond, TThird, TFourth, TReturn>(
        string routineName,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TReturn> map,
        dynamic? parameters = null
        );
    
    Task<IEnumerable<TReturn>> GetFromRoutineAsync<TFirst, TSecond, TThird, TFourth, TReturn>(
        string routineName,
        DbRoutineType? routineType,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TReturn> map,
        dynamic? parameters = null
        );
    
    Task<IEnumerable<TReturn>> GetFromRoutineAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(
        string routineName,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map,
        dynamic? parameters = null
        );
    
    Task<IEnumerable<TReturn>> GetFromRoutineAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(
        string routineName,
        DbRoutineType? routineType,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map,
        dynamic? parameters = null
        );
    
    Task<IEnumerable<TReturn>> GetFromRoutineAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(
        string routineName,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map,
        dynamic? parameters = null
        );
    
    Task<IEnumerable<TReturn>> GetFromRoutineAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(
        string routineName,
        DbRoutineType? routineType,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map,
        dynamic? parameters = null
        );
    
    Task<IEnumerable<TReturn>> GetFromRoutineAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(
        string routineName,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map,
        dynamic? parameters = null
        );
    
    Task<IEnumerable<TReturn>> GetFromRoutineAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(
        string routineName,
        DbRoutineType? routineType,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map,
        dynamic? parameters = null
        );
}