using Xunit.Sdk;
using Xunit.v3;

namespace Flowsy.Db.Unity.Test.Infrastructure.Testing.Ordering;

public sealed class OrderedTestCollectionOrderer : ITestCollectionOrderer
{
    public IReadOnlyCollection<TTestCollection> OrderTestCollections<TTestCollection>(
        IReadOnlyCollection<TTestCollection> testCollections)
        where TTestCollection : ITestCollection
        => testCollections
            .OrderBy(testCollection => OrderMetadata.GetCollectionOrder(testCollection))
            .ThenBy(testCollection => testCollection.TestCollectionDisplayName, StringComparer.Ordinal)
            .ToArray();
}
