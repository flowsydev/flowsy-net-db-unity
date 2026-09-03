namespace Flowsy.Db.Unity.Conventions;

/// <summary>
/// Represents conventions for database parameters.
/// </summary>
/// <param name="CaseStyle">The naming style to apply to parameter names.</param>
/// <param name="Prefix">Optional prefix to add to parameter names.</param>
/// <param name="Suffix">Optional suffix to add to parameter names.</param>
/// <param name="UseNamedParameters">Indicates whether to use named parameters in queries.</param>
/// <param name="Mappings">Optional explicit property-to-parameter mappings.</param>
public record DbParameterConvention(
    DbCaseStyle? CaseStyle = null,
    string? Prefix = null,
    string? Suffix = null,
    bool UseNamedParameters = false,
    IReadOnlyCollection<DbParameterMapping>? Mappings = null
) : DbObjectConvention(CaseStyle, Prefix, Suffix)
{
    private readonly System.Collections.Concurrent.ConcurrentDictionary<(Type, string), string> _resolvedNames = new();

    /// <summary>
    /// Default parameter convention with no specific naming style or parameters.
    /// </summary>
    public static readonly DbParameterConvention Default = new();

    /// <summary>
    /// Resolves an explicit property mapping before applying the default naming convention.
    /// </summary>
    public string ResolveName(Type? containerType, string propertyName)
    {
        if (containerType is null)
            return FormatName(propertyName);

        return _resolvedNames.GetOrAdd((containerType, propertyName), key =>
            Mappings?.FirstOrDefault(mapping =>
                mapping.ContainerType == key.Item1 && mapping.PropertyName == key.Item2)?.ParameterName
            ?? FormatName(key.Item2));
    }
}
