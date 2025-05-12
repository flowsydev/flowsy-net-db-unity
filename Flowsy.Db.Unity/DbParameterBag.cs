namespace Flowsy.Db.Unity;

public class DbParameterBag
{
    public DbParameterBag(DbParameterDescriptor descriptor, object? value)
    {
        Descriptor = descriptor;
        Value = value;
    }

    public DbParameterDescriptor Descriptor { get; }
    public object? Value { get; }
}