namespace Flowsy.Db.Unity.Test.Mock.Model;

public record CustomerAuditable(int CustomerId, string Name, string Email, AuditInfo AuditInfo);