namespace Flowsy.Db.Unity;

/// <summary>Validates and formats session settings for a provider.</summary>
public interface IDbSessionSettingFormatter
{
    /// <summary>Builds safe statements for applying and cleaning up the setting.</summary>
    DbSessionSettingCommand Format(DbSessionSetting setting, DbConnectionConfiguration configuration);
}
