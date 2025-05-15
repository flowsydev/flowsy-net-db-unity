using Flowsy.Core;

namespace Flowsy.Db.Unity.Conventions;

/// <summary>
/// Translates runtime type and member names to its corresponding database representation using the specified case style.
/// </summary>
public class DbEnumNameTranslator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DbEnumNameTranslator"/> class.
    /// </summary>
    /// <param name="memberNameCaseStyle">
    /// The case style to use for translating member names. If null, the original member name will be used.
    /// </param>
    /// <param name="typeNameCaseStyle">
    /// The case style to use for translating type names. If null, the original type name will be used.
    /// </param>
    public DbEnumNameTranslator(CaseStyle? memberNameCaseStyle = null, CaseStyle? typeNameCaseStyle = null)
    {
        MemberNameCaseStyle = memberNameCaseStyle;
        TypeNameCaseStyle = typeNameCaseStyle;
    }

    /// <summary>
    /// The case style to use for translating member names.
    /// </summary>
    public CaseStyle? MemberNameCaseStyle { get; internal set; }
    
    /// <summary>
    /// The case style to use for translating type names.
    /// </summary>
    public CaseStyle? TypeNameCaseStyle { get; internal set; }

    /// <summary>
    /// Translates the runtime member name to its corresponding database representation using the specified case style.
    /// </summary>
    /// <param name="memberName">
    /// The member name to translate.
    /// </param>
    /// <returns>
    /// The translated member name.
    /// </returns>
    public string TranslateMemberName(string memberName)
        => MemberNameCaseStyle.HasValue && !memberName.MatchesCaseStyle(MemberNameCaseStyle.Value) 
            ? memberName.ApplyCaseStyle(MemberNameCaseStyle)
            : memberName;
    
    
    /// <summary>
    /// Translates the runtime type name to its corresponding database representation using the specified case style.
    /// </summary>
    /// <param name="runtimeType">
    /// The runtime type for which the name is to be translated.
    /// </param>
    /// <returns>
    /// The translated type name.
    /// </returns>
    public string TranslateTypeName(Type runtimeType) => TranslateTypeName(runtimeType.Name);
    
    /// <summary>
    /// Translates the runtime type name to its corresponding database representation using the specified case style.
    /// </summary>
    /// <param name="runtimeTypeName">
    /// The runtime type name to translate.
    /// </param>
    /// <returns>
    /// The translated type name.
    /// </returns>
    public string TranslateTypeName(string runtimeTypeName)
        => TypeNameCaseStyle.HasValue && !runtimeTypeName.MatchesCaseStyle(TypeNameCaseStyle.Value) 
            ? runtimeTypeName.ApplyCaseStyle(TypeNameCaseStyle)
            : runtimeTypeName;
}