namespace Flowsy.Db.Unity;

public partial interface IDbAgent
{
    /// <summary>
    /// Executes a database statement (SQL command) and maps the result to multiple objects.
    /// </summary>
    /// <param name="commandText">
    /// The SQL command to execute.
    /// </param>
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the statement. This can be an anonymous object or a dictionary.
    /// </param>
    /// <typeparam name="TFirst">
    /// The type of the first object in the result set.
    /// </typeparam>
    /// <typeparam name="TSecond">
    /// The type of the second object in the result set.
    /// </typeparam>
    /// <typeparam name="TReturn">
    /// The type of the object to return.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of the mapped objects.
    /// </returns>
    IEnumerable<TReturn> GetFromStatement<TFirst, TSecond, TReturn>(
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TReturn> map,
        dynamic? parameters = null
        );
    
    /// <summary>
    /// Executes a database statement (SQL command) and maps the result to multiple objects.
    /// </summary>
    /// <param name="commandText">
    /// The SQL command to execute.
    /// </param>
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the statement. This can be an anonymous object or a dictionary.
    /// </param>
    /// <typeparam name="TFirst">
    /// The type of the first object in the result set.
    /// </typeparam>
    /// <typeparam name="TSecond">
    /// The type of the second object in the result set.
    /// </typeparam>
    /// <typeparam name="TThird">
    /// The type of the third object in the result set.
    /// </typeparam>
    /// <typeparam name="TReturn">
    /// The type of the object to return.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of the mapped objects.
    /// </returns>
    IEnumerable<TReturn> GetFromStatement<TFirst, TSecond, TThird, TReturn>(
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TReturn> map,
        dynamic? parameters = null
        );
    
    /// <summary>
    /// Executes a database statement (SQL command) and maps the result to multiple objects.
    /// </summary>
    /// <param name="commandText">
    /// The SQL command to execute.
    /// </param>
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the statement. This can be an anonymous object or a dictionary.
    /// </param>
    /// <typeparam name="TFirst">
    /// The type of the first object in the result set.
    /// </typeparam>
    /// <typeparam name="TSecond">
    /// The type of the second object in the result set.
    /// </typeparam>
    /// <typeparam name="TThird">
    /// The type of the third object in the result set.
    /// </typeparam>
    /// <typeparam name="TFourth">
    /// The type of the fourth object in the result set.
    /// </typeparam>
    /// <typeparam name="TReturn">
    /// The type of the object to return.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of the mapped objects.
    /// </returns>
    IEnumerable<TReturn> GetFromStatement<TFirst, TSecond, TThird, TFourth, TReturn>(
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TReturn> map,
        dynamic? parameters = null
        );
    
    /// <summary>
    /// Executes a database statement (SQL command) and maps the result to multiple objects.
    /// </summary>
    /// <param name="commandText">
    /// The SQL command to execute.
    /// </param>
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the statement. This can be an anonymous object or a dictionary.
    /// </param>
    /// <typeparam name="TFirst">
    /// The type of the first object in the result set.
    /// </typeparam>
    /// <typeparam name="TSecond">
    /// The type of the second object in the result set.
    /// </typeparam>
    /// <typeparam name="TThird">
    /// The type of the third object in the result set.
    /// </typeparam>
    /// <typeparam name="TFourth">
    /// The type of the fourth object in the result set.
    /// </typeparam>
    /// <typeparam name="TFifth">
    /// The type of the fifth object in the result set.
    /// </typeparam>
    /// <typeparam name="TReturn">
    /// The type of the object to return.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of the mapped objects.
    /// </returns>
    IEnumerable<TReturn> GetFromStatement<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map,
        dynamic? parameters = null
        );
    
    /// <summary>
    /// Executes a database statement (SQL command) and maps the result to multiple objects.
    /// </summary>
    /// <param name="commandText">
    /// The SQL command to execute.
    /// </param>
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the statement. This can be an anonymous object or a dictionary.
    /// </param>
    /// <typeparam name="TFirst">
    /// The type of the first object in the result set.
    /// </typeparam>
    /// <typeparam name="TSecond">
    /// The type of the second object in the result set.
    /// </typeparam>
    /// <typeparam name="TThird">
    /// The type of the third object in the result set.
    /// </typeparam>
    /// <typeparam name="TFourth">
    /// The type of the fourth object in the result set.
    /// </typeparam>
    /// <typeparam name="TFifth">
    /// The type of the fifth object in the result set.
    /// </typeparam>
    /// <typeparam name="TSixth">
    /// The type of the sixth object in the result set.
    /// </typeparam>
    /// <typeparam name="TReturn">
    /// The type of the object to return.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of the mapped objects.
    /// </returns>
    IEnumerable<TReturn> GetFromStatement<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map,
        dynamic? parameters = null
        );
    
    /// <summary>
    /// Executes a database statement (SQL command) and maps the result to multiple objects.
    /// </summary>
    /// <param name="commandText">
    /// The SQL command to execute.
    /// </param>
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the statement. This can be an anonymous object or a dictionary.
    /// </param>
    /// <typeparam name="TFirst">
    /// The type of the first object in the result set.
    /// </typeparam>
    /// <typeparam name="TSecond">
    /// The type of the second object in the result set.
    /// </typeparam>
    /// <typeparam name="TThird">
    /// The type of the third object in the result set.
    /// </typeparam>
    /// <typeparam name="TFourth">
    /// The type of the fourth object in the result set.
    /// </typeparam>
    /// <typeparam name="TFifth">
    /// The type of the fifth object in the result set.
    /// </typeparam>
    /// <typeparam name="TSixth">
    /// The type of the sixth object in the result set.
    /// </typeparam>
    /// <typeparam name="TSeventh">
    /// The type of the seventh object in the result set.
    /// </typeparam>
    /// <typeparam name="TReturn">
    /// The type of the object to return.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of the mapped objects.
    /// </returns>
    IEnumerable<TReturn> GetFromStatement<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map,
        dynamic? parameters = null
        );
    
    /// <summary>
    /// Asynchronously executes a database statement (SQL command) and maps the result to multiple objects.
    /// </summary>
    /// <param name="commandText">
    /// The SQL command to execute.
    /// </param>
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the statement. This can be an anonymous object or a dictionary.
    /// </param>
    /// <typeparam name="TFirst">
    /// The type of the first object in the result set.
    /// </typeparam>
    /// <typeparam name="TSecond">
    /// The type of the second object in the result set.
    /// </typeparam>
    /// <typeparam name="TReturn">
    /// The type of the object to return.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of the mapped objects.
    /// </returns>
    Task<IEnumerable<TReturn>> GetFromStatementAsync<TFirst, TSecond, TReturn>(
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TReturn> map,
        dynamic? parameters = null
        );
    
    /// <summary>
    /// Asynchronously executes a database statement (SQL command) and maps the result to multiple objects.
    /// </summary>
    /// <param name="commandText">
    /// The SQL command to execute.
    /// </param>
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the statement. This can be an anonymous object or a dictionary.
    /// </param>
    /// <typeparam name="TFirst">
    /// The type of the first object in the result set.
    /// </typeparam>
    /// <typeparam name="TSecond">
    /// The type of the second object in the result set.
    /// </typeparam>
    /// <typeparam name="TThird">
    /// The type of the third object in the result set.
    /// </typeparam>
    /// <typeparam name="TReturn">
    /// The type of the object to return.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of the mapped objects.
    /// </returns>
    Task<IEnumerable<TReturn>> GetFromStatementAsync<TFirst, TSecond, TThird, TReturn>(
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TReturn> map,
        dynamic? parameters = null
        );
    
    /// <summary>
    /// Asynchronously executes a database statement (SQL command) and maps the result to multiple objects.
    /// </summary>
    /// <param name="commandText">
    /// The SQL command to execute.
    /// </param>
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the statement. This can be an anonymous object or a dictionary.
    /// </param>
    /// <typeparam name="TFirst">
    /// The type of the first object in the result set.
    /// </typeparam>
    /// <typeparam name="TSecond">
    /// The type of the second object in the result set.
    /// </typeparam>
    /// <typeparam name="TThird">
    /// The type of the third object in the result set.
    /// </typeparam>
    /// <typeparam name="TFourth">
    /// The type of the fourth object in the result set.
    /// </typeparam>
    /// <typeparam name="TReturn">
    /// The type of the object to return.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of the mapped objects.
    /// </returns>
    Task<IEnumerable<TReturn>> GetFromStatementAsync<TFirst, TSecond, TThird, TFourth, TReturn>(
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TReturn> map,
        dynamic? parameters = null
        );
    
    /// <summary>
    /// Asynchronously executes a database statement (SQL command) and maps the result to multiple objects.
    /// </summary>
    /// <param name="commandText">
    /// The SQL command to execute.
    /// </param>
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the statement. This can be an anonymous object or a dictionary.
    /// </param>
    /// <typeparam name="TFirst">
    /// The type of the first object in the result set.
    /// </typeparam>
    /// <typeparam name="TSecond">
    /// The type of the second object in the result set.
    /// </typeparam>
    /// <typeparam name="TThird">
    /// The type of the third object in the result set.
    /// </typeparam>
    /// <typeparam name="TFourth">
    /// The type of the fourth object in the result set.
    /// </typeparam>
    /// <typeparam name="TFifth">
    /// The type of the fifth object in the result set.
    /// </typeparam>
    /// <typeparam name="TReturn">
    /// The type of the object to return.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of the mapped objects.
    /// </returns>
    Task<IEnumerable<TReturn>> GetFromStatementAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map,
        dynamic? parameters = null
        );
    
    /// <summary>
    /// Asynchronously executes a database statement (SQL command) and maps the result to multiple objects.
    /// </summary>
    /// <param name="commandText">
    /// The SQL command to execute.
    /// </param>
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the statement. This can be an anonymous object or a dictionary.
    /// </param>
    /// <typeparam name="TFirst">
    /// The type of the first object in the result set.
    /// </typeparam>
    /// <typeparam name="TSecond">
    /// The type of the second object in the result set.
    /// </typeparam>
    /// <typeparam name="TThird">
    /// The type of the third object in the result set.
    /// </typeparam>
    /// <typeparam name="TFourth">
    /// The type of the fourth object in the result set.
    /// </typeparam>
    /// <typeparam name="TFifth">
    /// The type of the fifth object in the result set.
    /// </typeparam>
    /// <typeparam name="TSixth">
    /// The type of the sixth object in the result set.
    /// </typeparam>
    /// <typeparam name="TReturn">
    /// The type of the object to return.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of the mapped objects.
    /// </returns>
    Task<IEnumerable<TReturn>> GetFromStatementAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map,
        dynamic? parameters = null
        );
    
    /// <summary>
    /// Asynchronously executes a database statement (SQL command) and maps the result to multiple objects.
    /// </summary>
    /// <param name="commandText">
    /// The SQL command to execute.
    /// </param>
    /// <param name="splitOn">
    /// A comma-separated list of column names to split the result set on.
    /// </param>
    /// <param name="map">
    /// A function that maps the result set to the desired object type.
    /// </param>
    /// <param name="parameters">
    /// The parameters to pass to the statement. This can be an anonymous object or a dictionary.
    /// </param>
    /// <typeparam name="TFirst">
    /// The type of the first object in the result set.
    /// </typeparam>
    /// <typeparam name="TSecond">
    /// The type of the second object in the result set.
    /// </typeparam>
    /// <typeparam name="TThird">
    /// The type of the third object in the result set.
    /// </typeparam>
    /// <typeparam name="TFourth">
    /// The type of the fourth object in the result set.
    /// </typeparam>
    /// <typeparam name="TFifth">
    /// The type of the fifth object in the result set.
    /// </typeparam>
    /// <typeparam name="TSixth">
    /// The type of the sixth object in the result set.
    /// </typeparam>
    /// <typeparam name="TSeventh">
    /// The type of the seventh object in the result set.
    /// </typeparam>
    /// <typeparam name="TReturn">
    /// The type of the object to return.
    /// </typeparam>
    /// <returns>
    /// An enumerable collection of the mapped objects.
    /// </returns>
    Task<IEnumerable<TReturn>> GetFromStatementAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(
        string commandText,
        string splitOn,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map,
        dynamic? parameters = null
        );
}