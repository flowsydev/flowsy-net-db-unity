namespace Flowsy.Db.Unity;

/// <summary>Validated statements for applying and cleaning up a session setting.</summary>
public sealed record DbSessionSettingCommand(string ApplyStatement, string CleanupStatement);
