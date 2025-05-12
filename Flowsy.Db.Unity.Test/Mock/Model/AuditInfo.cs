namespace Flowsy.Db.Unity.Test.Mock.Model;

public record AuditInfo(DateTimeOffset CreatedAt, DateTimeOffset? UpdatedAt);