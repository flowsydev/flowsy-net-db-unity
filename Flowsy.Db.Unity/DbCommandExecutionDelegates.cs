using Dapper;

namespace Flowsy.Db.Unity;

/// <summary>
/// Represents a method that will be called before a database command is executed.
/// </summary>
public delegate void DbCommandPreExecutionHandler(CommandDefinition commandDefinition);

/// <summary>
/// Represents a method that will be called after a database command is executed.
/// </summary>
/// <typeparam name="T"></typeparam>
public delegate T DbCommandPostExecutionHandler<T>(CommandDefinition commandDefinition, T result);

/// <summary>
/// Represents a method that will be called asynchronously after a database command is executed.
/// </summary>
/// <typeparam name="T"></typeparam>
public delegate Task<T> DbCommandPostExecutionAsyncHandler<T>(CommandDefinition commandDefinition, T result);
