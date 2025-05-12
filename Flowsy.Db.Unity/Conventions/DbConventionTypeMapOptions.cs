using System.Collections.Concurrent;
using Flowsy.Core;

namespace Flowsy.Db.Unity.Conventions;

public class DbConventionTypeMapOptions
{
    public ConcurrentDictionary<string, DbConventionTypeMapGroup> TypeGroups { get; set; } = [];
    public bool StrictMode { get; set; }

    public void AddTypeGroup(CaseStyle? columnCaseStyle, params Type[] types)
        => AddTypeGroup(columnCaseStyle, null, null, types);

    public void AddTypeGroup(CaseStyle? columnCaseStyle, string? columnPrefix, params Type[] types)
        => AddTypeGroup(columnCaseStyle, columnPrefix, null, types);
    
    public void AddTypeGroup(CaseStyle? columnCaseStyle, string? columnPrefix, string? columnSuffix, params Type[] types)
        => AddTypeGroup(new DbObjectNameConvention(columnCaseStyle, columnPrefix, columnSuffix), types);
    
    public void AddTypeGroup(DbObjectNameConvention columnNameConvention, params Type[] types)
    {
        var columnCaseStyle = columnNameConvention.CaseStyle;
        var columnPrefix = columnNameConvention.Prefix;
        var columnSuffix = columnNameConvention.Suffix;
        var groupKey = $"{columnCaseStyle.ToString()}:{columnPrefix}:{columnSuffix}";
        
        TypeGroups.TryGetValue(groupKey, out var group);
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
            TypeGroups[groupKey] = group;
        }
        foreach (var type in types)
        {
            if (group.Types.Contains(type)) continue;
            group.Types.Add(type);
        }
    }
}