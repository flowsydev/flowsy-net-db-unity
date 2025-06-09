using Flowsy.Db.Unity.Extensions;
using Flowsy.Db.Unity.Test.Extensions;
using Flowsy.Db.Unity.Test.Mock;
using Flowsy.Db.Unity.Test.Mock.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit.Abstractions;
using Xunit.Extensions.Ordering;

namespace Flowsy.Db.Unity.Test.Scenarios;

[Collection(Collections.DependencyInjection), Order(4)]
public class S07PrimaryUnitOfWorkTest
{
    private readonly ServiceHost _serviceHost;
    private readonly ITestOutputHelper _output;

    public S07PrimaryUnitOfWorkTest(ServiceHost serviceHost, ITestOutputHelper output)
    {
        _serviceHost = serviceHost;
        _output = output;
    }

    [Fact, Order(1)]
    public async Task T01_Should_Involve_PrimaryAgent()
    {
        // Arrange
        Exception? exception = null;
        
        // Act
        try
        {
            await using var scope = _serviceHost.ServiceProvider.CreateAsyncScope();

            var optionsSnapshot = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<DbConnectionOptions>>();
            var connectionOptions = optionsSnapshot.Get("Primary");
            
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IDbPrimaryUnitOfWork>();
            _output.Subscribe(unitOfWork);
            
            var customerRepository = scope.ServiceProvider.GetRequiredService<IPrimaryCustomerRepository>();
            _output.Subscribe(customerRepository.Agent);
            
            unitOfWork.BeginWork();
            
            unitOfWork.Involve(customerRepository);
            await customerRepository.CreateCustomerAsync("John Lennon", "john.lennon@thebeatles.com", CustomerStatus.Active, DateTimeOffset.Now);
            await customerRepository.CreateCustomerAsync("Paul McCartney", "paul.mccartney@thebeatles.com", CustomerStatus.Active, DateTimeOffset.Now);
            
            await unitOfWork.InvolveAsync(async (c, t, ct) =>
            {
                await c.ExecuteRoutineAsync("crm.cst_create", new
                    {
                        Name = "George Harrison",
                        Email = "george.harrison@thebeatles.com",
                        Status = (CustomerStatus?) CustomerStatus.Active,
                        CreatedAt = DateTimeOffset.Now,
                    },
                    t,
                    conventions: connectionOptions.Conventions,
                    cancellationToken: ct
                    );
                
                await c.ExecuteRoutineAsync("crm.cst_create", new
                    {
                        Name = "Ringo Starr",
                        Email = "ringo.starr@thebeatles.com",
                        Status = (CustomerStatus?) CustomerStatus.Active,
                        CreatedAt = DateTimeOffset.Now,
                    },
                    t,
                    conventions: connectionOptions.Conventions,
                    cancellationToken: ct
                    );
            });
        
            await unitOfWork.CompleteWorkAsync();
        }
        catch (Exception e)
        {
            exception = e;
        }

        // Assert
        exception.ShouldBeNull();
    }
    
    [Fact, Order(2)]
    public async Task T02_Should_GetResults_FromRoutine_Async()
    {
        // Arrange
        await using var scope = _serviceHost.ServiceProvider.CreateAsyncScope();
        var agent = scope.ServiceProvider.GetRequiredService<IDbPrimaryAgent>();
        _output.Subscribe(agent);
        
        // Act
        IList<Customer>? customers = null;
        Exception? exception = null;
        try
        {
            customers = (await agent.GetFromRoutineAsync<Customer>(
                "crm.cst_get_by_filter",
                new
                {
                    SearchTerm = "@thebeatles.com",
                    Status = (CustomerStatus?) CustomerStatus.Active,
                }
            )).ToList();
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        // Assert
        exception.ShouldBeNull();
        customers.ShouldNotBeNull();
        customers.Count.ShouldBe(4);
    }
}