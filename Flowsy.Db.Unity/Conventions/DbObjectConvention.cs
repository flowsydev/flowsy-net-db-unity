namespace Flowsy.Db.Unity.Conventions;

/// <summary>
/// Represents conventions for database objects (tables, columns, etc.).
/// </summary>
/// <param name="CaseStyle">The naming style to apply to object names.</param>
/// <param name="Prefix">Optional prefix to add to object names.</param>
/// <param name="Suffix">Optional suffix to add to object names.</param>
public record DbObjectConvention(
    DbCaseStyle? CaseStyle = null,
    string? Prefix = null,
    string? Suffix = null
) : DbConvention
{
    /// <summary>
    /// Formats an object name according to the configured conventions.
    /// </summary>
    /// <param name="objectName">The original object name to format.</param>
    /// <returns>The formatted object name with the applied naming style, prefix, and suffix.</returns>
    public virtual string FormatName(string objectName)
    {
        var finalProvider = ConventionSet?.Provider ?? DbProviderDescriptor.Generic;
        var fqn = new DbFullyQualifiedName(finalProvider, objectName);

        var parts = fqn.Parts.ToArray();
        if (parts.Length == 0)
            return string.Empty;
        
        var finalCaseStyle = CaseStyle ?? ConventionSet?.DefaultCaseStyle ?? DbCaseStyle.None;
        var prefix = string.IsNullOrWhiteSpace(Prefix) ? string.Empty : Prefix;
        var suffix = string.IsNullOrWhiteSpace(Suffix) ? string.Empty : Suffix;
        
        return string.Join(finalProvider.ObjectSeparator, parts.Select((p, i) =>
        {
            var part = finalCaseStyle.Apply(p);
            return i == parts.Length - 1
                ? $"{prefix}{part}{suffix}"
                : part;
        }));
    }
}