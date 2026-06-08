using System.Globalization;
using System.Reflection;
using Dapper;

namespace Flowsy.Db.Unity.Test.Extensions;

public static class OutputExtensions
{
    private static string DividerLineSingle => new string('-', 100);
    private static string DividerLineDouble => new string('=', 100);
    
    public static void WriteDivider(this ITestOutputHelper outputHelper, bool singleLine = true)
    {
        outputHelper.WriteLine(singleLine ? DividerLineSingle : DividerLineDouble);
    }
    
    public static void WriteHeader(this ITestOutputHelper outputHelper, string message, bool doubleLine = true)
    {
        outputHelper.WriteDivider(!doubleLine);
        outputHelper.WriteLine(message);
        outputHelper.WriteDivider(!doubleLine);
    }
    
    public static void WriteEmptyLine(this ITestOutputHelper outputHelper)
    {
        outputHelper.WriteLine(string.Empty);
    }
    
    public static void Write(this ITestOutputHelper outputHelper, IDictionary<string, object?> dictionary, string? header = null, bool headerDoubleLine = true)
    {
        if (!string.IsNullOrEmpty(header))
            outputHelper.WriteHeader(header, headerDoubleLine);

        foreach (var (k, v) in dictionary)
        {
            var finalValue = v switch
            {
                DateTimeOffset dto => FormatDateTime(dto.ToLocalTime().DateTime),
                DateTime dt => FormatDateTime(dt.ToLocalTime()),
                _ => v
            };
            
            outputHelper.WriteLine($"{k}: {finalValue}");
        }
    }
    
    private static string FormatDateTime(DateTime dateTime)
    {
        var date = dateTime.ToString("yyyy-MM-dd");
        var time = dateTime.ToString("HH:mm:ss.fffffff");
        var offset = dateTime.ToString("zzz");
        return $"{date} {time} {offset}";
    }
    
    public static void Write(this ITestOutputHelper outputHelper, IEnumerable<IDictionary<string, object?>> dictionaries, string? header = null)
    {
        if (!string.IsNullOrEmpty(header))
            outputHelper.WriteHeader(header);

        var itemCount = 0;
        foreach (var d in dictionaries)
        {
            itemCount++;
            outputHelper.Write(d, $"Item #{itemCount:N0}", string.IsNullOrEmpty(header));
            outputHelper.WriteEmptyLine();
        }
    }

    public static void Write(this ITestOutputHelper outputHelper, CommandDefinition commandDefinition, object? result = null, string? header = null)
    {
        outputHelper.WriteHeader(header ?? "Command Definition");
        outputHelper.WriteLine("Command Text: {0}", commandDefinition.CommandText);
        outputHelper.WriteLine($"Command Type: {commandDefinition.CommandType}");
        outputHelper.WriteLine($"Command Timeout: {commandDefinition.CommandTimeout}");
        outputHelper.WriteLine("Command Flags: {0}", commandDefinition.Flags);
        outputHelper.WriteLine($"Transaction: {commandDefinition.Transaction}");

        var parameters = commandDefinition.Parameters;
        IDictionary<string, object?> dictionary = new Dictionary<string, object?>();
        if (parameters is DynamicParameters dp)
            foreach (var name in dp.ParameterNames) dictionary.Add(name, dp.Get<object?>(name));
        else if (parameters is IDictionary<string, object?> d)
            dictionary = d;
        else if (parameters is not null)
        {
            var type = parameters.GetType();
            var properties = type.GetRuntimeProperties();
            foreach (var property in properties)
            {
                var value = property.GetValue(parameters);
                dictionary.Add(property.Name, value);
            }
        }
        var parameterString = string.Join(", ", dictionary.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
        outputHelper.WriteLine("Parameters: {{ {0} }}", parameterString);

        if (result is not null)
        {
            var r = result switch
            {
                byte byteValue => byteValue.ToString(),
                short shortValue => shortValue.ToString(),
                int intValue => intValue.ToString(),
                long longValue => longValue.ToString(),
                float floatValue => floatValue.ToString(CultureInfo.InvariantCulture),
                double doubleValue => doubleValue.ToString(CultureInfo.InvariantCulture),
                decimal decimalValue => decimalValue.ToString(CultureInfo.InvariantCulture),
                bool boolValue => boolValue.ToString(),
                DateTime dateTimeValue => dateTimeValue.ToString("yyyy-MM-dd HH:mm:ss.fffffff"),
                DateTimeOffset dateTimeOffsetValue => dateTimeOffsetValue.ToString("yyyy-MM-dd HH:mm:ss.fffffff zzz"),
                Guid guidValue => guidValue.ToString(),
                IEnumerable<byte> intEnumerable => string.Join(", ", intEnumerable),
                IEnumerable<short> intEnumerable => string.Join(", ", intEnumerable),
                IEnumerable<int> intEnumerable => string.Join(", ", intEnumerable),
                IEnumerable<long> longEnumerable => string.Join(", ", longEnumerable),
                IEnumerable<float> floatEnumerable => string.Join(", ", floatEnumerable),
                IEnumerable<double> doubleEnumerable => string.Join(", ", doubleEnumerable),
                IEnumerable<decimal> decimalEnumerable => string.Join(", ", decimalEnumerable),
                IEnumerable<string> stringEnumerable => string.Join(", ", stringEnumerable),
                IEnumerable<object> enumerable => string.Join(", ", enumerable),
                _ => result?.ToString() ?? "<null>"
            };
            outputHelper.WriteLine("Result: {0}", r);
        }
        outputHelper.WriteEmptyLine();
    }

    public static T Write<T>(this ITestOutputHelper outputHelper, CommandDefinition commandDefinition, T result, string? header = null)
    {
        outputHelper.Write(commandDefinition, result as object, header);
        return result;
    }
}
