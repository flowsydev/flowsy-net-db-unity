using System.Collections.Concurrent;
using Flowsy.Core;

namespace Flowsy.Db.Unity.Conventions;

/// <summary>
/// Allows to configure a set of naming conventions for mapping type members to database columns when executing queries.
/// </summary>
public class DbConventionTypeMapOptions
{
    private ConcurrentDictionary<string, DbConventionTypeMapGroup> TypeGroupDictionary { get; set; } = [];
    
    /// <summary>
    /// A collection of type groups that define the mapping conventions for different types.
    /// </summary>
    public IEnumerable<DbConventionTypeMapGroup> TypeGroups => TypeGroupDictionary.Values;

    /// <summary>
    /// A flag indicating whether strict mode is enabled.
    /// Strict mode ensures that all columns found in query results are mapped to members of the given type.
    /// </summary>
    public bool StrictMode { get; set; }

    /// <summary>
    /// Adds a type group to the collection.
    /// </summary>
    /// <param name="columnCaseStyle">
    /// The case style to be used for matching columns with type members.
    /// </param>
    /// <param name="types">
    /// One or more types to be included in the type group.
    /// </param>
    public void AddTypeGroup(CaseStyle? columnCaseStyle, params Type[] types)
        => AddTypeGroup(columnCaseStyle, null, null, types);

    /// <summary>
    /// Adds a type group to the collection.
    /// </summary>
    /// <param name="columnCaseStyle">
    /// The case style to be used for matching columns with type members.
    /// </param>
    /// <param name="columnPrefix">
    /// The prefix to be used for matching columns with type members.
    /// </param>
    /// <param name="types">
    /// One or more types to be included in the type group.
    /// </param>
    public void AddTypeGroup(CaseStyle? columnCaseStyle, string? columnPrefix, params Type[] types)
        => AddTypeGroup(columnCaseStyle, columnPrefix, null, types);
    
    
    /// <summary>
    /// Adds a type group to the collection.
    /// </summary>
    /// <param name="columnCaseStyle">
    /// The case style to be used for matching columns with type members.
    /// </param>
    /// <param name="columnPrefix">
    /// The prefix to be used for matching columns with type members.
    /// </param>
    /// <param name="columnSuffix">
    /// The suffix to be used for matching columns with type members.
    /// </param>
    /// <param name="types">
    /// One or more types to be included in the type group.
    /// </param>
    public void AddTypeGroup(CaseStyle? columnCaseStyle, string? columnPrefix, string? columnSuffix, params Type[] types)
        => AddTypeGroup(new DbObjectNameConvention(columnCaseStyle, columnPrefix, columnSuffix), types);
    
    /// <summary>
    /// Adds a type group to the collection.
    /// </summary>
    /// <param name="columnNameConvention">
    /// The naming convention to be used for matching columns with type members.
    /// </param>
    /// <param name="types">
    /// One or more types to be included in the type group.
    /// </param>
    public void AddTypeGroup(DbObjectNameConvention columnNameConvention, params Type[] types)
    {
        var columnCaseStyle = columnNameConvention.CaseStyle;
        var columnPrefix = columnNameConvention.Prefix;
        var columnSuffix = columnNameConvention.Suffix;
        var groupKey = $"{columnCaseStyle.ToString()}:{columnPrefix}:{columnSuffix}";
        
        TypeGroupDictionary.TryGetValue(groupKey, out var group);
        if (group is null)
        {
            group = new DbConventionTypeMapGroup
            {
                ColumnNaming = new DbObjectNameConvention
                {
                    CaseStyle = columnCaseStyle,
                    Prefix = columnPrefix,
                    Suffix = columnSuffix
                }
            };
            TypeGroupDictionary[groupKey] = group;
        }
        foreach (var type in types)
        {
            if (group.Types.Contains(type)) continue;
            group.Types.Add(type);
        }
    }
}