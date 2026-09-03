using System.Globalization;
using System.Text.RegularExpressions;

namespace Flowsy.Db.Unity;

/// <summary>Conservative formatter with explicit allowlists per provider family.</summary>
public sealed partial class DbSessionSettingFormatter : IDbSessionSettingFormatter
{
    private static readonly IReadOnlyDictionary<DbProviderFamily, IReadOnlySet<string>> Defaults =
        new Dictionary<DbProviderFamily, IReadOnlySet<string>>
        {
            [DbProviderFamily.Postgres] = Set("application_name", "search_path", "statement_timeout", "lock_timeout", "timezone"),
            [DbProviderFamily.SqlServer] = Set("deadlock_priority", "lock_timeout"),
            [DbProviderFamily.MySql] = Set("sql_mode", "time_zone"),
            [DbProviderFamily.Oracle] = Set("current_schema"),
            [DbProviderFamily.Sqlite] = Set("foreign_keys", "busy_timeout")
        };

    /// <inheritdoc />
    public DbSessionSettingCommand Format(DbSessionSetting setting, DbConnectionConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(setting);
        if (!Identifier().IsMatch(setting.Name))
            throw new ArgumentException("The setting name is not a valid identifier.", nameof(setting));
        var defaults = Defaults.GetValueOrDefault(configuration.Provider.Family);
        if (!(defaults?.Contains(setting.Name) ?? false) && !configuration.AllowedSessionSettings.Contains(setting.Name))
            throw new InvalidOperationException($"Setting '{setting.Name}' is not allowed for connection '{configuration.ConnectionKey}'.");

        var value = FormatValue(setting.Value);
        return configuration.Provider.Family switch
        {
            DbProviderFamily.Postgres => new($"SET {setting.Name} TO {value}", $"RESET {setting.Name}"),
            DbProviderFamily.SqlServer => new($"SET {setting.Name} {value}", $"SET {setting.Name} DEFAULT"),
            DbProviderFamily.MySql => new($"SET SESSION {setting.Name} = {value}", $"SET SESSION {setting.Name} = DEFAULT"),
            DbProviderFamily.Oracle => new($"ALTER SESSION SET {setting.Name} = {value}", $"ALTER SESSION SET {setting.Name} = DEFAULT"),
            DbProviderFamily.Sqlite => new($"PRAGMA {setting.Name} = {value}", $"PRAGMA {setting.Name} = 0"),
            _ => throw new NotSupportedException($"No session-setting formatter is available for {configuration.Provider.Family}.")
        };
    }

    private static string FormatValue(object? value) => value switch
    {
        null => "NULL",
        bool boolean => boolean ? "1" : "0",
        byte or sbyte or short or ushort or int or uint or long or ulong or float or double or decimal
            => Convert.ToString(value, CultureInfo.InvariantCulture)!,
        Enum @enum => $"'{@enum.ToString().Replace("'", "''", StringComparison.Ordinal)}'",
        _ => $"'{Convert.ToString(value, CultureInfo.InvariantCulture)!.Replace("'", "''", StringComparison.Ordinal)}'"
    };

    private static IReadOnlySet<string> Set(params string[] values) => new HashSet<string>(values, StringComparer.OrdinalIgnoreCase);

    [GeneratedRegex("^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.CultureInvariant)]
    private static partial Regex Identifier();
}
