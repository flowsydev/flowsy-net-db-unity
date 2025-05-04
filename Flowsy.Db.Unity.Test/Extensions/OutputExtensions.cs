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
}