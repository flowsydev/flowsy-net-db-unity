namespace Flowsy.Db.Unity.Test.Infrastructure.Testing.Ordering;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class OrderAttribute(int value) : Attribute
{
    public int Value { get; } = value;
}
