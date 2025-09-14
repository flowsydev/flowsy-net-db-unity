namespace Flowsy.Db.Unity.Test.Mock.Model;

public record OperationContext(OperationContextUserAccount UserAccount, OperationContextServiceAccount ServiceAccount);

public record OperationContextUserAccount(Guid Id, string PrincipalName, string Email, string Nickname);

public record OperationContextServiceAccount(Guid Id, string ClientId);