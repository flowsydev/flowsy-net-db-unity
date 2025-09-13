namespace Flowsy.Db.Unity;

/// <summary>
/// Specifies the different supported database provider families.
/// </summary>
public enum DbProviderFamily
{
    /// <summary>
    /// Generic database provider that does not belong to a specific family.
    /// </summary>
    Generic,
    
    /// <summary>
    /// Provider family for PostgreSQL databases.
    /// </summary>
    Postgres,
    
    /// <summary>
    /// Provider family for MySQL and MariaDB databases.
    /// </summary>
    MySql,
    
    /// <summary>
    /// Provider family for Microsoft SQL Server databases.
    /// </summary>
    SqlServer,
    
    /// <summary>
    /// Provider family for Oracle Database.
    /// </summary>
    Oracle,
    
    /// <summary>
    /// Provider family for SQLite databases.
    /// </summary>
    Sqlite
}