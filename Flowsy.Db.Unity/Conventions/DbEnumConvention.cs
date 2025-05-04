using Flowsy.Core;
using Flowsy.Db.Unity.Resources;

namespace Flowsy.Db.Unity.Conventions;

public class DbEnumConvention : DbConvention
{
    public DbEnumConvention(DbConventionSet conventions) : base(conventions)
    {
    }

    public DbEnumFormat Format { get; internal set; } = DbEnumFormat.Name;
    public CaseStyle? CaseStyle { get; internal set; }
    
    private readonly List<DbEnumMapping> _mappings = [];
    public IEnumerable<DbEnumMapping> Mappings => _mappings;
    
    internal void AddMapping(DbEnumMapping mapping)
    {
        if (_mappings.Any(m => m.RuntimeType == mapping.RuntimeType))
            throw new InvalidOperationException(string.Format(Strings.MappingForEnumTypeXAlreadyExists, mapping.RuntimeType));
        
        _mappings.Add(mapping);
    }

    public void CopyTo(DbEnumConvention other)
    {
        other.Format = Format;
        other.CaseStyle = CaseStyle;
        
        foreach (var mapping in Mappings)
        {
            var newMapping = new DbEnumMapping(mapping.RuntimeType, mapping.DatabaseTypeName, Conventions);
            other.AddMapping(newMapping);
        }
    }

    public DbEnumConvention Clone()
    {
        var clone = new DbEnumConvention(Conventions);
        CopyTo(clone);
        return clone;
    }
}