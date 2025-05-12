using Flowsy.Core;

namespace Flowsy.Db.Unity.Conventions;

public class DbObjectNameConvention
{
    public DbObjectNameConvention()
    {
    }

    public DbObjectNameConvention(CaseStyle? caseStyle, string? prefix, string? suffix)
    {
        CaseStyle = caseStyle;
        Prefix = prefix;
        Suffix = suffix;
    }

    public CaseStyle? CaseStyle { get; set; }
    public string? Prefix { get; set; }
    public string? Suffix { get; set; }
}