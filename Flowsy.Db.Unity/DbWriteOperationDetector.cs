namespace Flowsy.Db.Unity;

/// <summary>Basic detector for common SQL write statements.</summary>
public sealed class DbWriteOperationDetector : IDbWriteOperationDetector
{
    private static readonly string[] WriteKeywords =
        ["INSERT", "UPDATE", "DELETE", "MERGE", "REPLACE", "UPSERT", "TRUNCATE", "CREATE", "ALTER", "DROP", "GRANT", "REVOKE"];

    /// <inheritdoc />
    public bool IsWriteOperation(string statement)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(statement);
        var span = statement.AsSpan().TrimStart();
        while (span.StartsWith("--"))
        {
            var end = span.IndexOfAny('\r', '\n');
            if (end < 0)
                return false;
            span = span[(end + 1)..].TrimStart();
        }

        foreach (var keyword in WriteKeywords)
        {
            if (span.StartsWith(keyword, StringComparison.OrdinalIgnoreCase)
                && (span.Length == keyword.Length || !char.IsLetterOrDigit(span[keyword.Length])))
                return true;
        }
        return false;
    }
}
