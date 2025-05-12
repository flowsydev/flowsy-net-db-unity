using Dapper;

namespace Flowsy.Db.Unity;

public delegate void DbCommandPreExecutionHandler(CommandDefinition commandDefinition);
public delegate T DbCommandPostExecutionHandler<T>(CommandDefinition commandDefinition, T result);
public delegate Task<T> DbCommandPostExecutionAsyncHandler<T>(CommandDefinition commandDefinition, T result);

public delegate void DbCommandExecutingEventHandler(object sender, DbCommandExecutingEventArgs e);
public delegate void DbCommandExecutedEventHandler(object sender, DbCommandExecutedEventArgs e);