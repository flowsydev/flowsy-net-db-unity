namespace Flowsy.Db.Unity.Test.Mock.Infrastructure.Time;

public static class Clock
{
    public static DateTimeOffset GetTimestamp() => DateTimeOffset.Now;
}