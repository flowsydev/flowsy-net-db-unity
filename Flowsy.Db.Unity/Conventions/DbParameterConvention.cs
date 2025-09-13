namespace Flowsy.Db.Unity.Conventions;

/// <summary>
/// Represents conventions for database parameters.
/// </summary>
/// <param name="CaseStyle">The naming style to apply to parameter names.</param>
/// <param name="Prefix">Optional prefix to add to parameter names.</param>
/// <param name="Suffix">Optional suffix to add to parameter names.</param>
/// <param name="UseNamedParameters">Indicates whether to use named parameters in queries.</param>
public record DbParameterConvention(
    DbCaseStyle? CaseStyle = null,
    string? Prefix = null,
    string? Suffix = null,
    bool UseNamedParameters = false
) : DbObjectConvention(CaseStyle, Prefix, Suffix)
{
    /// <summary>
    /// Default parameter convention with no specific naming style or parameters.
    /// </summary>
    public static readonly DbParameterConvention Default = new();
}