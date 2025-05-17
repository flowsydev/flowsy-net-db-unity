using Flowsy.Core;

namespace Flowsy.Db.Unity.Conventions;

/// <summary>
/// Builder for configuring parameter conventions.
/// </summary>
public class DbParameterConventionBuilder : DbConventionBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DbParameterConventionBuilder"/> class.
    /// </summary>
    /// <param name="parent">
    /// The parent <see cref="DbConventionSetBuilder"/> instance. This is used to access the parent convention set and apply configurations.
    /// </param>
    internal DbParameterConventionBuilder(DbConventionSetBuilder parent) : base(parent)
    {
    }
    
    /// <summary>
    /// Configures the naming convention for parameters.
    /// </summary>
    /// <param name="caseStyle">
    /// The case style to use for the parameter names. If null, the default case style for the convention set will be used.
    /// </param>
    /// <param name="prefix">
    /// The prefix to use for the parameter names. If null, no prefix will be added.
    /// </param>
    /// <param name="suffix">
    /// The suffix to use for the parameter names. If null, no suffix will be added.
    /// </param>
    /// <returns>
    /// The current instance of <see cref="DbParameterConventionBuilder"/> for method chaining.
    /// </returns>
    public DbParameterConventionBuilder UseNames(CaseStyle? caseStyle, string? prefix = null, string? suffix = null)
    {
        Parent.Conventions.Parameters.Naming.CaseStyle = caseStyle;
        Parent.Conventions.Parameters.Naming.Prefix = prefix ?? string.Empty;
        Parent.Conventions.Parameters.Naming.Suffix = suffix ?? string.Empty;
        return this;
    }
    
    /// <summary>
    /// Configures the case style for parameter names.
    /// </summary>
    /// <param name="caseStyle">
    /// The case style to use for the parameter names. If null, the default case style for the convention set will be used.
    /// </param>
    /// <returns>
    /// The current instance of <see cref="DbParameterConventionBuilder"/> for method chaining.
    /// </returns>
    public DbParameterConventionBuilder UseCaseStyle(CaseStyle? caseStyle)
    {
        Parent.Conventions.Parameters.Naming.CaseStyle = caseStyle;
        return this;
    }

    /// <summary>
    /// Configures the prefix for parameter names.
    /// </summary>
    /// <param name="prefix">
    /// The prefix to use for the parameter names. If null, no prefix will be added.
    /// </param>
    /// <returns></returns>
    public DbParameterConventionBuilder UsePrefix(string prefix)
    {
        Parent.Conventions.Parameters.Naming.Prefix = prefix;
        return this;
    }
    
    /// <summary>
    /// Configures the suffix for parameter names.
    /// </summary>
    /// <param name="suffix">
    /// The suffix to use for the parameter names. If null, no suffix will be added.
    /// </param>
    /// <returns>
    /// The current instance of <see cref="DbParameterConventionBuilder"/> for method chaining.
    /// </returns>
    public DbParameterConventionBuilder UseSuffix(string suffix)
    {
        Parent.Conventions.Parameters.Naming.Suffix = suffix;
        return this;
    }
}