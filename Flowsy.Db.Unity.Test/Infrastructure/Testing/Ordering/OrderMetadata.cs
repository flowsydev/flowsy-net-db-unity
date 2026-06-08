using System.Reflection;
using Xunit.Sdk;

namespace Flowsy.Db.Unity.Test.Infrastructure.Testing.Ordering;

internal static class OrderMetadata
{
    public const int DefaultOrder = 0;

    public static int GetClassOrder(ITestCase testCase) => GetOrder(ResolveType(testCase.TestClassName));

    public static int GetMethodOrder(ITestCase testCase)
    {
        var testClass = ResolveType(testCase.TestClassName);
        if (testClass is null || testCase.TestMethodName is null)
            return DefaultOrder;

        return testClass
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(method => method.Name == testCase.TestMethodName)
            .Select(GetOrder)
            .DefaultIfEmpty(DefaultOrder)
            .Min();
    }

    public static int GetCollectionOrder(ITestCollection testCollection)
        => GetOrder(ResolveType(testCollection.TestCollectionClassName));

    private static int GetOrder(MemberInfo? member)
        => member?.GetCustomAttribute<OrderAttribute>(inherit: false)?.Value ?? DefaultOrder;

    private static Type? ResolveType(string? typeName)
        => string.IsNullOrWhiteSpace(typeName)
            ? null
            : Type.GetType(typeName) ?? AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType(typeName))
                .FirstOrDefault(type => type is not null);
}
