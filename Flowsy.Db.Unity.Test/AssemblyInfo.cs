using Flowsy.Db.Unity.Test.Mock;
using Flowsy.Db.Unity.Test.Infrastructure.Testing.Ordering;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

[assembly: TestCollectionOrderer(typeof(OrderedTestCollectionOrderer))]

[assembly: TestCaseOrderer(typeof(OrderedTestCaseOrderer))]

[assembly: AssemblyFixture(typeof(ServiceHost))]
