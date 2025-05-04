using Flowsy.Core;
using Flowsy.Db.Unity.Conventions;

namespace Flowsy.Db.Unity.Configuration;

public class DbEnumConventionBuilder
{
    private readonly DbConventionSetBuilder _conventionSetBuilder;

    public DbEnumConventionBuilder(DbConventionSetBuilder conventionSetBuilder)
    {
        _conventionSetBuilder = conventionSetBuilder;
    }
    
    public DbEnumConventionBuilder UseFormat(DbEnumFormat format)
    {
        _conventionSetBuilder.ConventionSet.Enums.Format = format;
        return this;
    }
    
    public DbEnumConventionBuilder UseCaseStyle(CaseStyle? caseStyle)
    {
        _conventionSetBuilder.ConventionSet.Enums.CaseStyle = caseStyle;
        return this;
    }

    public DbEnumConventionBuilder UseMapping<T>(string databaseTypeName) where T : struct, Enum
    {
        _conventionSetBuilder.ConventionSet.Enums.AddMapping(new DbEnumMapping<T>(databaseTypeName, _conventionSetBuilder.ConventionSet));
        return this;
    }

    public DbEnumConventionBuilder UseMappings(params DbEnumMapping[] mappings)
    {
        foreach (var mapping in mappings)
            _conventionSetBuilder.ConventionSet.Enums.AddMapping(mapping);
        
        return this;
    }
    
    public DbRoutineConventionBuilder ForRoutines() => _conventionSetBuilder.ForRoutines();
    public DbParameterConventionBuilder ForParameters() => _conventionSetBuilder.ForParameters();
    public DbEnumConventionBuilder ForEnums() => _conventionSetBuilder.ForEnums();
}