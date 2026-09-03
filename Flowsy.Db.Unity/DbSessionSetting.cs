namespace Flowsy.Db.Unity;

/// <summary>Temporary provider-neutral value applied to a connection context.</summary>
public sealed record DbSessionSetting(string Name, object? Value);
