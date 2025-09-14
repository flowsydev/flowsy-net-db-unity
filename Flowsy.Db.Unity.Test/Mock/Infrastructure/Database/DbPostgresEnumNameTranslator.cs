using Npgsql;

namespace Flowsy.Db.Unity.Test.Mock.Infrastructure.Database;

public class DbPostgresEnumNameTranslator : INpgsqlNameTranslator
{
    private readonly DbEnumMapping _enumMapping;
    private readonly DbEnumNameTranslator? _fallbackTranslator;

    public DbPostgresEnumNameTranslator(DbEnumMapping enumMapping, DbEnumNameTranslator? fallbackTranslator = null)
    {
        _enumMapping = enumMapping;
        _fallbackTranslator = fallbackTranslator;
    }

    public string TranslateTypeName(string clrName)
    {
        var translator = _enumMapping.NameTranslator ?? _fallbackTranslator;
        return !string.IsNullOrEmpty(_enumMapping.DatabaseTypeName) 
            ? _enumMapping.DatabaseTypeName
            : translator?.TranslateTypeName(clrName) ?? clrName;
    }

    public string TranslateMemberName(string clrName)
    {
        var translator = _enumMapping.NameTranslator ?? _fallbackTranslator;
        return translator?.TranslateMemberName(clrName) ?? clrName;
    }
}