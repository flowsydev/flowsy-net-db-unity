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
            [DbProviderFamily.Db2] = Set("current_schema"),
            [DbProviderFamily.Sqlite] = Set("foreign_keys", "busy_timeout")
        };

    /// <inheritdoc />
    public DbSessionSettingCommand Format(DbSessionSetting setting, DbConnectionConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(setting);
        ArgumentNullException.ThrowIfNull(configuration);
        if (!Identifier().IsMatch(setting.Name))
            throw new ArgumentException("The setting name is not a valid identifier.", nameof(setting));
        var defaults = Defaults.GetValueOrDefault(configuration.Provider.Family);
        if (!(defaults?.Contains(setting.Name) ?? false) && !configuration.AllowedSessionSettings.Contains(setting.Name))
            throw new InvalidOperationException($"Setting '{setting.Name}' is not allowed for connection '{configuration.ConnectionKey}'.");

        var name = setting.Name.ToLowerInvariant();
        return configuration.Provider.Family switch
        {
            DbProviderFamily.Postgres => CreateCommand(
                setting.Value,
                $"SELECT current_setting('{name}')",
                value => $"SELECT set_config('{name}', {QuoteText(value)}, false)",
                $"RESET {name}"),
            DbProviderFamily.MySql => CreateCommand(
                setting.Value,
                $"SELECT @@SESSION.{name}",
                value => $"SET SESSION {name} = {FormatValue(value)}",
                $"SET SESSION {name} = DEFAULT"),
            DbProviderFamily.Sqlite => CreateCommand(
                setting.Value,
                $"PRAGMA {name}",
                value => $"PRAGMA {name} = {FormatValue(value)}",
                $"PRAGMA {name} = 0"),
            DbProviderFamily.SqlServer when name is "deadlock_priority" or "lock_timeout" => CreateCommand(
                setting.Value,
                $"SELECT {name} FROM sys.dm_exec_sessions WHERE session_id = @@SPID",
                value => $"SET {name} {FormatSqlServerValue(name, value)}",
                $"SET {name} {(name == "lock_timeout" ? -1 : 0)}"),
            DbProviderFamily.Oracle when name == "current_schema" => CreateCommand(
                setting.Value,
                "SELECT SYS_CONTEXT('USERENV', 'CURRENT_SCHEMA') FROM DUAL",
                value => $"ALTER SESSION SET CURRENT_SCHEMA = {QuoteIdentifier(value)}",
                "BEGIN EXECUTE IMMEDIATE 'ALTER SESSION SET CURRENT_SCHEMA = \"' || REPLACE(SYS_CONTEXT('USERENV', 'SESSION_USER'), '\"', '\"\"') || '\"'; END;"),
            DbProviderFamily.Db2 when name == "current_schema" => CreateCommand(
                setting.Value,
                "VALUES CURRENT SCHEMA",
                value => $"SET SCHEMA {QuoteText(value)}",
                "SET SCHEMA USER"),
            _ => throw new NotSupportedException(
                $"Setting '{setting.Name}' for {configuration.Provider.Family} requires a custom session-setting formatter.")
        };
    }

    private static DbSessionSettingCommand CreateCommand(
        object? value,
        string readStatement,
        Func<object?, string> apply,
        string cleanupStatement)
        => new(apply(value), cleanupStatement)
        {
            ReadStatement = readStatement,
            RestoreStatementFactory = apply
        };

    private static string FormatSqlServerValue(string name, object? value)
    {
        if (name == "deadlock_priority" && value is string priority
            && priority.ToUpperInvariant() is "LOW" or "NORMAL" or "HIGH")
            return priority.ToUpperInvariant();
        var text = Convert.ToString(value, CultureInfo.InvariantCulture);
        if (!int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var number)
            || (name == "deadlock_priority" && number is < -10 or > 10)
            || (name == "lock_timeout" && number < -1))
            throw new ArgumentException($"The value for setting '{name}' is not valid.", nameof(value));
        return number.ToString(CultureInfo.InvariantCulture);
    }

    private static string QuoteIdentifier(object? value)
    {
        var text = Convert.ToString(value, CultureInfo.InvariantCulture);
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        return $"\"{text.Replace("\"", "\"\"", StringComparison.Ordinal)}\"";
    }

    private static string QuoteText(object? value)
        => value is null ? "NULL" : $"'{Convert.ToString(value, CultureInfo.InvariantCulture)!.Replace("'", "''", StringComparison.Ordinal)}'";

    private static string FormatValue(object? value) => value switch
    {
        null => "NULL",
        bool boolean => boolean ? "1" : "0",
        byte or sbyte or short or ushort or int or uint or long or ulong or float or double or decimal
            => Convert.ToString(value, CultureInfo.InvariantCulture)!,
        _ => QuoteText(value)
    };

    private static IReadOnlySet<string> Set(params string[] values) => new HashSet<string>(values, StringComparer.OrdinalIgnoreCase);

    [GeneratedRegex("^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.CultureInvariant)]
    private static partial Regex Identifier();
}
