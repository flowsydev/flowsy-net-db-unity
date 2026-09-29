using System.Data;
using Dapper;

namespace Flowsy.Db.Unity;

/// <summary>Fallback opt-in que representa DateOnly como DateTime.</summary>
public sealed class DbDateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    /// <inheritdoc />
    public override void SetValue(IDbDataParameter parameter, DateOnly value)
    {
        parameter.DbType = DbType.Date;
        parameter.Value = value.ToDateTime(TimeOnly.MinValue);
    }

    /// <inheritdoc />
    public override DateOnly Parse(object value) => value switch
    {
        DateOnly date => date,
        DateTime dateTime => DateOnly.FromDateTime(dateTime),
        string text when DateOnly.TryParse(text, System.Globalization.CultureInfo.InvariantCulture, out var date) => date,
        string text => DateOnly.FromDateTime(DateTime.Parse(text, System.Globalization.CultureInfo.InvariantCulture)),
        _ => throw new DataException($"Cannot convert {value.GetType()} to DateOnly.")
    };
}
