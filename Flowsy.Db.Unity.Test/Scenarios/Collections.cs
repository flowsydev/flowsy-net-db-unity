using Xunit.Extensions.Ordering;

namespace Flowsy.Db.Unity.Test.Scenarios;

public static class Collections
{
    public const string ConventionSet = "ConventionSet";
    public const string DependencyInjection = "DependencyInjection";
}

[CollectionDefinition(Collections.ConventionSet), Order(1)]
public sealed class ConventionSetCollection;

[CollectionDefinition(Collections.DependencyInjection), Order(99)]
public sealed class DependencyInjectionCollection;