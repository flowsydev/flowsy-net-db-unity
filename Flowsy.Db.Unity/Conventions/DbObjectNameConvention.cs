using Flowsy.Core;

namespace Flowsy.Db.Unity.Conventions;

/// <summary>
/// Represents a naming convention for database objects.
/// </summary>
public class DbObjectNameConvention
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DbObjectNameConvention"/> class.
    /// </summary>
    public DbObjectNameConvention()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DbObjectNameConvention"/> class with the specified case style, prefix, and suffix.
    /// </summary>
    /// <param name="caseStyle">
    /// The case style to be used for the name.
    /// </param>
    /// <param name="prefix">
    /// The prefix to be added to the name.
    /// </param>
    /// <param name="suffix">
    /// The suffix to be added to the name.
    /// </param>
    public DbObjectNameConvention(CaseStyle? caseStyle, string? prefix, string? suffix)
    {
        CaseStyle = caseStyle;
        Prefix = prefix;
        Suffix = suffix;
    }

    /// <summary>
    /// Gets or sets the case style to be used for the name.
    /// </summary>
    public CaseStyle? CaseStyle { get; set; }
    
    /// <summary>
    /// Gets or sets the prefix to be added to the name.
    /// </summary>
    public string? Prefix { get; set; }
    
    /// <summary>
    /// Gets or sets the suffix to be added to the name.
    /// </summary>
    public string? Suffix { get; set; }
}