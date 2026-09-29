using System.Data;
using Dapper;

namespace Flowsy.Db.Unity;

/// <summary>Options that override command conventions for a single call.</summary>
public sealed record DbSessionCallOptions
{
    /// <summary>Command timeout, in seconds.</summary>
    public int? Timeout { get; init; }

    /// <summary>Command type sent to the provider.</summary>
    public CommandType? CommandType { get; init; }

    /// <summary>Dapper execution flags.</summary>
    public CommandFlags? Flags { get; init; }

    /// <summary>Correlation tag; it is never appended to SQL.</summary>
    public string? Tag { get; init; }

    internal string? SanitizedTag => string.IsNullOrWhiteSpace(Tag)
        ? null
        : new string(Tag.Trim().Where(c => char.IsLetterOrDigit(c) || c is '-' or '_' or '.' or ':').Take(128).ToArray());
}
