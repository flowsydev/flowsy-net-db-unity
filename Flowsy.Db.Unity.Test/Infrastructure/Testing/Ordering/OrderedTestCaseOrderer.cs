using Xunit.Sdk;
using Xunit.v3;

namespace Flowsy.Db.Unity.Test.Infrastructure.Testing.Ordering;

public sealed class OrderedTestCaseOrderer : ITestCaseOrderer
{
    public IReadOnlyCollection<TTestCase> OrderTestCases<TTestCase>(IReadOnlyCollection<TTestCase> testCases)
        where TTestCase : notnull, ITestCase
        => testCases
            .OrderBy(testCase => OrderMetadata.GetClassOrder(testCase))
            .ThenBy(testCase => OrderMetadata.GetMethodOrder(testCase))
            .ThenBy(testCase => testCase.TestCaseDisplayName, StringComparer.Ordinal)
            .ToArray();
}
