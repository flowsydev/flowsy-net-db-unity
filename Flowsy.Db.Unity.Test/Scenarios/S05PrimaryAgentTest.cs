using Flowsy.Db.Unity.Test.Extensions;
using Flowsy.Db.Unity.Test.Mock;
using Flowsy.Db.Unity.Test.Mock.Model;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Abstractions;
using Xunit.Extensions.Ordering;

namespace Flowsy.Db.Unity.Test.Scenarios;

[Collection(Collections.DependencyInjection), Order(2)]
public class S05PrimaryAgentTest
{
    private readonly ServiceHost _serviceHost;
    private readonly GlobalModel _globalModel;
    private readonly ITestOutputHelper _output;

    public S05PrimaryAgentTest(ServiceHost serviceHost, GlobalModel globalModel, ITestOutputHelper output)
    {
        _serviceHost = serviceHost;
        _globalModel = globalModel;
        _output = output;
    }

    [Fact, Order(1)]
    public void T01_Should_GetResults_FromRoutine_Sync()
    {
        // Arrange
        using var scope = _serviceHost.ServiceProvider.CreateScope();
        var agent = scope.ServiceProvider.GetRequiredService<IDbPrimaryAgent>();
        _output.Subscribe(agent);

        // Act
        Customer[]? customers = null;
        Exception? exception = null;
        try
        {
            customers = agent.GetFromRoutine<Customer>(
                "crm.cst_get_by_filter",
                new {SearchTerm = "@example.com"}
            ).ToArray();
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        // Assert
        exception.ShouldBeNull();
        customers.ShouldNotBeNull();
        customers.ShouldNotBeEmpty();
    }

    [Fact, Order(2)]
    public async Task T02_Should_Perform_MultiMapQuery()
    {
        // Arrange
        await using var scope = _serviceHost.ServiceProvider.CreateAsyncScope();
        var agent = scope.ServiceProvider.GetRequiredService<IDbPrimaryAgent>();
        _output.Subscribe(agent);
        
        // Act
        IList<CustomerAuditable>? customers = null;
        Exception? exception = null;
        try
        {
            customers = (
                await agent.GetFromRoutineAsync<dynamic, dynamic, CustomerAuditable>(
                    "crm.cst_get_by_filter",
                    "created_at",
                    (c, a) => new CustomerAuditable(
                        c.customer_id, c.name, c.email,
                        new AuditInfo(
                            new DateTimeOffset((DateTime) a.created_at),
                            a.modified_at != null ? new DateTimeOffset((DateTime) a.modified_at) : null
                            )
                    ),
                    new {SearchTerm = "@example.com"}
                )
            ).ToArray();
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        // Assert
        exception.ShouldBeNull();
        customers.ShouldNotBeNull();
        customers.ShouldNotBeEmpty();
    }
}