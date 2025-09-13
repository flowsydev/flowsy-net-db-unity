using Flowsy.Db.Unity.Test.Mock;
using Flowsy.Db.Unity.Test.Scenarios;
using Xunit.Extensions.Ordering;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

[assembly: TestFramework("Xunit.Extensions.Ordering.TestFramework", "Xunit.Extensions.Ordering")]

[assembly: TestCollectionOrderer("Xunit.Extensions.Ordering.CollectionOrderer", "Xunit.Extensions.Ordering")]

[assembly: TestCaseOrderer("Xunit.Extensions.Ordering.TestCaseOrderer", "Xunit.Extensions.Ordering")]

[assembly: AssemblyFixture(typeof(ServiceHost))]
