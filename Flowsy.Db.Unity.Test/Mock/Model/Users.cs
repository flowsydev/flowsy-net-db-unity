namespace Flowsy.Db.Unity.Test.Mock.Model;

public enum UserAccountStatus
{
    Enabled,
    Locked,
    Suspended,
    Disabled
}

public record UserAccountOverview(
    Guid UserAccountId, string PrincipalName, string Email, string FirstName, string LastName, string Nickname 
    ) : IReadModel;

public record UserAccountDetail(
    Guid UserAccountId, string PrincipalName, string Email, string FirstName, string LastName, string Nickname,
    string? PasswordHash, AuditTrace CreationTrace, AuditTrace? LastMutationTrace
    ) : IReadModel;
