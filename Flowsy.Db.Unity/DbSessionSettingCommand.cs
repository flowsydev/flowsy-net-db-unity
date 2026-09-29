namespace Flowsy.Db.Unity;

/// <summary>Validated statements for applying and cleaning up a session setting.</summary>
public sealed record DbSessionSettingCommand(string ApplyStatement, string CleanupStatement)
{
    /// <summary>Gets the scalar query used to capture the current value before applying the setting.</summary>
    public string? ReadStatement { get; init; }

    /// <summary>
    /// Gets the factory that builds a restoration statement from the captured value.
    /// Supply this together with <see cref="ReadStatement"/>. When neither is supplied,
    /// the session uses <see cref="CleanupStatement"/> for cleanup.
    /// </summary>
    public Func<object?, string>? RestoreStatementFactory { get; init; }
}
