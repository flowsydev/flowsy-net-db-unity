using Flowsy.Core;

namespace Flowsy.Db.Unity.Conventions;

public class DbEnumNameTranslator
{
    public DbEnumNameTranslator(CaseStyle? memberNameCaseStyle = null, CaseStyle? typeNameCaseStyle = null)
    {
        MemberNameCaseStyle = memberNameCaseStyle;
        TypeNameCaseStyle = typeNameCaseStyle;
    }

    public CaseStyle? MemberNameCaseStyle { get; internal set; }
    public CaseStyle? TypeNameCaseStyle { get; internal set; }

    public string TranslateMemberName(string memberName)
        => MemberNameCaseStyle.HasValue && !memberName.MatchesCaseStyle(MemberNameCaseStyle.Value) 
            ? memberName.ApplyCaseStyle(MemberNameCaseStyle)
            : memberName;
    
    public string TranslateTypeName(Type runtimeType) => TranslateTypeName(runtimeType.Name);
    
    public string TranslateTypeName(string runtimeTypeName)
        => TypeNameCaseStyle.HasValue && !runtimeTypeName.MatchesCaseStyle(TypeNameCaseStyle.Value) 
            ? runtimeTypeName.ApplyCaseStyle(TypeNameCaseStyle)
            : runtimeTypeName;
}