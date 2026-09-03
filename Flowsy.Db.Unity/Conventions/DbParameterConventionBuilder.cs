using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Flowsy.Db.Unity.Conventions;

/// <summary>Builds parameter conventions and external mappings.</summary>
public sealed partial class DbParameterConventionBuilder
{
    private DbCaseStyle? _caseStyle;
    private string? _prefix;
    private string? _suffix;
    private bool _useNamedParameters;
    private readonly List<DbParameterMapping> _mappings = [];

    /// <summary>Defines the convention applied to properties without an explicit mapping.</summary>
    public DbParameterConventionBuilder WithDefault(
        DbCaseStyle? caseStyle = null,
        string? prefix = null,
        string? suffix = null,
        bool useNamedParameters = false)
    {
        _caseStyle = caseStyle;
        _prefix = prefix;
        _suffix = suffix;
        _useNamedParameters = useNamedParameters;
        return this;
    }

    /// <summary>Maps a readable property to an explicit identifier.</summary>
    public DbParameterConventionBuilder Map<TParameters, TProperty>(
        Expression<Func<TParameters, TProperty>> property,
        string parameterName)
    {
        ArgumentNullException.ThrowIfNull(property);
        if (property.Body is not MemberExpression { Member: PropertyInfo propertyInfo } || !propertyInfo.CanRead)
            throw new ArgumentException("The expression must select a readable public property.", nameof(property));
        if (!ValidIdentifier().IsMatch(parameterName))
            throw new ArgumentException("The parameter name must be a valid identifier.", nameof(parameterName));

        _mappings.RemoveAll(x => x.ContainerType == typeof(TParameters) && x.PropertyName == propertyInfo.Name);
        _mappings.Add(new DbParameterMapping(typeof(TParameters), propertyInfo.Name, parameterName));
        return this;
    }

    internal DbParameterConvention Build()
        => new(_caseStyle, _prefix, _suffix, _useNamedParameters, _mappings.ToArray());

    [GeneratedRegex("^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.CultureInvariant)]
    private static partial Regex ValidIdentifier();
}
