namespace Flowsy.Db.Unity.Test.Mock;

public interface IPrimaryCustomerRepository : IDbUnitOfWorkParticipant
{
    IDbPrimaryAgent Agent { get; }
    Task CreateCustomerAsync(string name, string email, DateTimeOffset createdAt, CancellationToken cancellationToken = default);
}

public class PrimaryCustomerRepository : DbUnitOfWorkParticipant, IPrimaryCustomerRepository
{
    public PrimaryCustomerRepository(IDbPrimaryAgent agent)
    {
        Agent = agent;
    }
    
    public IDbPrimaryAgent Agent { get; }

    public Task CreateCustomerAsync(string name, string email, DateTimeOffset createdAt, CancellationToken cancellationToken = default)
        => Agent.ExecuteRoutineAsync("crm.cst_create", new
        {
            Name = name,
            Email = email,
            CreatedAt = createdAt,
        }, cancellationToken);

    public override IDbUnitOfWork? UnitOfWork => Agent.UnitOfWork;

    public override void JoinWork(IDbUnitOfWork unitOfWork) => Agent.JoinWork(unitOfWork);

    public override void DetachFromWork() => Agent.DetachFromWork();
}