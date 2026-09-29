using System.Data;
using Dapper;

namespace Flowsy.Db.Unity;

/// <summary>Fallback opt-in que representa TimeOnly como TimeSpan.</summary>
public sealed class DbTimeOnlyTypeHandler : SqlMapper.TypeHandler<TimeOnly>
{
    /// <inheritdoc />
    public override void SetValue(IDbDataParameter parameter, TimeOnly value)
    {
        parameter.DbType = DbType.Time;
        parameter.Value = value.ToTimeSpan();
    }

    /// <inheritdoc />
    public override TimeOnly Parse(object value) => value switch
    {
        TimeOnly time => time,
        TimeSpan timeSpan => TimeOnly.FromTimeSpan(timeSpan),
        DateTime dateTime => TimeOnly.FromDateTime(dateTime),
        string text => TimeOnly.Parse(text, System.Globalization.CultureInfo.InvariantCulture),
        _ => throw new DataException($"Cannot convert {value.GetType()} to TimeOnly.")
    };
}
