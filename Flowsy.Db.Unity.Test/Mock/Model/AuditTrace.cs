namespace Flowsy.Db.Unity.Test.Mock.Model;

public record AuditTrace(DateTimeOffset Instant, OperationContext Context);