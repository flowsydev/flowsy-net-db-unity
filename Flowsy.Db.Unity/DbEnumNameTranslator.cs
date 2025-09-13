namespace Flowsy.Db.Unity;

/// <summary>
/// Provides functionality to translate enum type names and their members using different naming styles.
/// </summary>
public class DbEnumNameTranslator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DbEnumNameTranslator"/> class.
    /// </summary>
    /// <param name="memberNameCaseStyle">
    /// Naming style to apply to enum member names.
    /// If <c>null</c>, no transformation is applied to member names.
    /// </param>
    /// <param name="typeNameCaseStyle">
    /// Naming style to apply to enum type names.
    /// If <c>null</c>, no transformation is applied to type names.
    /// </param>
    public DbEnumNameTranslator(DbCaseStyle? memberNameCaseStyle = null, DbCaseStyle? typeNameCaseStyle = null)
    {
        MemberNameCaseStyle = memberNameCaseStyle;
        TypeNameCaseStyle = typeNameCaseStyle;
    }

    /// <summary>
    /// Gets the naming style that is applied to enum member names.
    /// </summary>
    public DbCaseStyle? MemberNameCaseStyle { get; init; }
    
    /// <summary>
    /// Gets the naming style that is applied to enum type names.
    /// </summary>
    public DbCaseStyle? TypeNameCaseStyle { get; init; }

    /// <summary>
    /// Translates an enum member name by applying the configured naming style.
    /// </summary>
    /// <param name="memberName">
    /// The original enum member name to translate.
    /// </param>
    /// <returns>
    /// The translated member name with the naming style applied.
    /// If no naming style has been configured, returns the original name.
    /// </returns>
    public string TranslateMemberName(string memberName)
        => MemberNameCaseStyle.HasValue && MemberNameCaseStyle.Value != DbCaseStyle.None 
            ? MemberNameCaseStyle.Value.Apply(memberName)
            : memberName;
    
    /// <summary>
    /// Translates an enum type name by applying the configured naming style.
    /// </summary>
    /// <param name="runtimeType">
    /// The enum type whose name is to be translated.
    /// </param>
    /// <returns>
    /// The translated type name with the naming style applied.
    /// If no naming style has been configured, returns the original type name.
    /// </returns>
    public string TranslateTypeName(Type runtimeType) => TranslateTypeName(runtimeType.Name);
    
    /// <summary>
    /// Translates an enum type name by applying the configured naming style.
    /// </summary>
    /// <param name="runtimeTypeName">
    /// The original enum type name to translate.
    /// </param>
    /// <returns>
    /// The translated type name with the naming style applied.
    /// If no naming style has been configured, returns the original name.
    /// </returns>
    public string TranslateTypeName(string runtimeTypeName)
        => TypeNameCaseStyle.HasValue && TypeNameCaseStyle.Value != DbCaseStyle.None
            ? TypeNameCaseStyle.Value.Apply(runtimeTypeName)
            : runtimeTypeName;
}