using System.Globalization;
using System.Reflection;
using Dapper;
using Flowsy.Db.Unity.Extensions;
using Xunit.Abstractions;

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

    public static void WriteFullPath(this ITestOutputHelper outputHelper, DbFullyQualifiedName fullyQualifiedName)
    {
        var root = fullyQualifiedName;
        while (root.Parent is not null)
            root = root.Parent;

        outputHelper.Write(root);
    }

    public static void Write(this ITestOutputHelper outputHelper, DbFullyQualifiedName fullyQualifiedName)
    {
        var next = fullyQualifiedName;
        while (true)
        {
            outputHelper.WriteHeader(next.ToString());

            foreach (var part in next.Parts)
                outputHelper.WriteLine("Part: {0}", part);

            if (next.Child is null) return;

            outputHelper.WriteEmptyLine();
            next = next.Child;
        }
    }

    public static void Write(this ITestOutputHelper outputHelper, DbRoutineDescriptor routineDescriptor)
    {
        outputHelper.WriteHeader("Routine Description");
        outputHelper.WriteLine("Provider: {0}", routineDescriptor.Provider.Family);
        outputHelper.WriteLine("Name: {0}", routineDescriptor.FullyQualifiedName);
        outputHelper.WriteLine("Type: {0}", routineDescriptor.Type);
        outputHelper.WriteLine("Is Procedure: {0}", routineDescriptor.IsProcedure);
        outputHelper.WriteLine("Is Function: {0}", routineDescriptor.IsFunction);
        outputHelper.WriteLine("Command Text: {0}", routineDescriptor.CommandText);
        outputHelper.WriteLine("Command Type: {0}", routineDescriptor.CommandType);
        outputHelper.WriteLine("Returns Table: {0}", routineDescriptor.ReturnsTable);
        outputHelper.Write(routineDescriptor.Parameters);
        outputHelper.WriteEmptyLine();
    }

    public static void Write(this ITestOutputHelper outputHelper, IEnumerable<DbParameterDescriptor> parameterDescriptors)
    {
        var parameters = parameterDescriptors.ToArray();
        outputHelper.WriteLine("Parameters ({0}):", parameters.Length);
        foreach (var parameter in parameters)
            Write(outputHelper, parameter, false, 2);
        
        outputHelper.WriteEmptyLine();
    }
    
    public static void Write(this ITestOutputHelper outputHelper, DbParameterDescriptor parameterDescriptor, bool includeHeader = false, int indent = 0)
    {
        if (includeHeader)
            outputHelper.WriteHeader("Parameter Description");
        
        var indentation = new string(' ', indent);
        outputHelper.WriteLine("{0}Provider: {1}", indentation, parameterDescriptor.Provider.Family);
        outputHelper.WriteLine("{0}Name: {1}", indentation, parameterDescriptor.Name);
        outputHelper.WriteLine("{0}Runtime Type: {1}", indentation, parameterDescriptor.RuntimeType.Name);
        outputHelper.WriteLine("{0}Database Type: {1}", indentation, parameterDescriptor.DatabaseType);
        outputHelper.WriteLine("{0}Custom Type: {1}", indentation, parameterDescriptor.CustomType);
        outputHelper.WriteLine("{0}Direction: {1}", indentation, parameterDescriptor.Direction);
        outputHelper.WriteLine("{0}Size: {1}", indentation, parameterDescriptor.Size);
        outputHelper.WriteLine("{0}Precision: {1}", indentation, parameterDescriptor.Precision);
        outputHelper.WriteLine("{0}Scale: {1}", indentation, parameterDescriptor.Scale);
        outputHelper.WriteEmptyLine();
    }

    public static void Write(this ITestOutputHelper outputHelper, CommandDefinition commandDefinition, object? result = null, string? header = null)
    {
        outputHelper.WriteHeader(header ?? "Command Definition");
        outputHelper.WriteLine("Command Text: {0}", commandDefinition.CommandText);
        outputHelper.WriteLine("Command Type: {0}", commandDefinition.CommandType);
        outputHelper.WriteLine("Command Timeout: {0}", commandDefinition.CommandTimeout);
        outputHelper.WriteLine("Command Flags: {0}", commandDefinition.Flags);
        outputHelper.WriteLine("Transaction: {0}", commandDefinition.Transaction);

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

    public static void Subscribe(this ITestOutputHelper outputHelper, IDbAgent dbAgent)
    {
        dbAgent.CommandExecuting += (sender, e) =>
        {
            outputHelper.Write(e.CommandDefinition, null, "Command Executing");
        };
        
        dbAgent.CommandExecuted += (sender, e) =>
        {
            outputHelper.Write(e.CommandDefinition, e.Result, "Command Executed");
        };
    }

    public static void Subscribe(this ITestOutputHelper outputHelper, IDbUnitOfWork unitOfWork)
    {
        unitOfWork.WorkBegun += (sender, e) =>
        {
            var uow = (IDbUnitOfWork)sender!;
            outputHelper.WriteHeader("Unit of Work Begun");
            outputHelper.WriteLine("Database: {0}", uow.Connection.GetDatabaseUrl());
            outputHelper.WriteEmptyLine();
        };
        
        unitOfWork.WorkCompleted += (sender, e) =>
        {
            var uow = (IDbUnitOfWork)sender!;
            outputHelper.WriteHeader("Unit of Work Completed");
            outputHelper.WriteLine("Database: {0}", uow.Connection.GetDatabaseUrl());
            outputHelper.WriteEmptyLine();
        };
        
        unitOfWork.WorkDiscarded += (sender, e) =>
        {
            var uow = (IDbUnitOfWork)sender!;
            outputHelper.WriteHeader("Unit of Work Discarded");
            outputHelper.WriteLine("Database: {0}", uow.Connection.GetDatabaseUrl());
            outputHelper.WriteEmptyLine();
        };
    }
}