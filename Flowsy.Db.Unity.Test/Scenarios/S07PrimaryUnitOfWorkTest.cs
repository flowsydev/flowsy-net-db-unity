using Flowsy.Db.Unity.Test.Extensions;
using Flowsy.Db.Unity.Test.Mock;
using Flowsy.Db.Unity.Test.Mock.Model;
using Microsoft.Extensions.DependencyInjection;
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
        await using var scope = _serviceHost.ServiceProvider.CreateAsyncScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IDbPrimaryUnitOfWork>();
        _output.Subscribe(unitOfWork);
        
        
        // Act
        Exception? exception = null;
        try
        {
            var agent = unitOfWork.InvolveService<IDbPrimaryAgent>();
            _output.Subscribe(agent);
            
            unitOfWork.BeginWork();

            await agent.ExecuteRoutineAsync("crm.cst_create", new
            {
                Name = "John Lennon",
                Email = "john.lennon@thebeatles.com",
                CreatedAt = DateTimeOffset.Now,
            });

            await agent.ExecuteRoutineAsync("crm.cst_create", new
            {
                Name = "Paul McCartney",
                Email = "paul.mccartney@thebeatles.com",
                CreatedAt = DateTimeOffset.Now,
            });

            await agent.ExecuteRoutineAsync("crm.cst_create", new
            {
                Name = "George Harrison",
                Email = "george.harrison@thebeatles.com",
                CreatedAt = DateTimeOffset.Now,
            });

            await agent.ExecuteRoutineAsync("crm.cst_create", new
            {
                Name = "Ringo Starr",
                Email = "ringo.starr@thebeatles.com",
                CreatedAt = DateTimeOffset.Now,
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
                    SearchTerm = "@thebeatles.com"
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