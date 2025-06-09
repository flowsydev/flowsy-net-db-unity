using Flowsy.Db.Unity.Test.Mock.Model;

namespace Flowsy.Db.Unity.Test.Mock;

public interface IPrimaryCustomerRepository : IDbUnitOfWorkParticipant
{
    IDbPrimaryAgent Agent { get; }
    Task CreateCustomerAsync(string name, string email, CustomerStatus status, DateTimeOffset createdAt, CancellationToken cancellationToken = default);
}

public class PrimaryCustomerRepository : DbUnitOfWorkParticipant, IPrimaryCustomerRepository
{
    public PrimaryCustomerRepository(IDbPrimaryAgent agent)
    {
        Agent = agent;
    }
    
    public IDbPrimaryAgent Agent { get; }

    public Task CreateCustomerAsync(string name, string email, CustomerStatus status, DateTimeOffset createdAt, CancellationToken cancellationToken = default)
        => Agent.ExecuteRoutineAsync("crm.cst_create", new
        {
            Name = name,
            Email = email,
            Status = status,
            CreatedAt = createdAt,
        }, cancellationToken);

    public override IDbUnitOfWork? UnitOfWork => Agent.UnitOfWork;

    public override void Join(IDbUnitOfWork unitOfWork) => Agent.Join(unitOfWork);

    public override void Leave() => Agent.Leave();
}