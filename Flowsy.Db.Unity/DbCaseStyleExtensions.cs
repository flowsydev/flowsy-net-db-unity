using CaseON;

namespace Flowsy.Db.Unity;

/// <summary>
/// Provides extension methods for applying naming styles to text strings.
/// </summary>
public static class DbCaseStyleExtensions
{
    /// <summary>
    /// Applies the specified naming style to a text string.
    /// </summary>
    /// <param name="caseStyle">
    /// The naming style to apply.
    /// </param>
    /// <param name="value">
    /// The text string to which the naming style will be applied.
    /// </param>
    /// <returns>
    /// The text string with the naming style applied.
    /// If the style is <see cref="DbCaseStyle.None"/>, returns the original string without modifications.
    /// </returns>
    public static string Apply(this DbCaseStyle caseStyle, string value)
        => caseStyle switch
        {
            DbCaseStyle.LowerSnakeCase => ConvertON.ToSnakeCase(value),
            DbCaseStyle.UpperSnakeCase => ConvertON.ToSnakeCase(value).ToUpperInvariant(),
            DbCaseStyle.KebabCase => ConvertON.ToKebabCase(value),
            DbCaseStyle.TrainCase => ConvertON.ToTitleCase(value).Replace(" ", "-"),
            DbCaseStyle.CamelCase => ConvertON.ToCamelCase(value),
            DbCaseStyle.PascalCase => ConvertON.ToPascalCase(value),
            DbCaseStyle.TitleCase => ConvertON.ToTitleCase(value),
            DbCaseStyle.SentenceCase => ConvertON.ToSentenceCase(value),
            _ => value
        };
}